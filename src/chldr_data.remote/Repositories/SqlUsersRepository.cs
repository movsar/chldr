﻿using chldr_data.DatabaseObjects.Models;
using chldr_data.DatabaseObjects.Dtos;
using chldr_tools;
using chldr_data.Enums;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_data.Interfaces.Repositories;
using chldr_utils.Services;
using chldr_data.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using chldr_shared.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using chldr_data.Resources.Localizations;
using Realms.Sync;

namespace chldr_data.remote.Repositories
{
    public class SqlUsersRepository : SqlRepository<SqlUser, UserModel, UserDto>, IUsersRepository
    {
        private readonly SignInManager<SqlUser> _signInManager;
        private readonly UserManager<SqlUser> _userManager;
        private readonly EmailService _emailService;
        private readonly IStringLocalizer<AppLocalizations> _localizer;

        public SqlUsersRepository(
            DbContextOptions<SqlContext> dbConfig,
            FileService fileService,
            UserManager<SqlUser> userManager,
            SignInManager<SqlUser> signInManager,
            EmailService emailService,
            IStringLocalizer<AppLocalizations> localizer,
            string _userId) : base(dbConfig, fileService, _userId)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _localizer = localizer;
        }

        protected override RecordType RecordType => RecordType.User;
        public async Task SetStatusAsync(string userId, UserStatus newStatus)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId) as SqlUser;
            if (user == null)
            {
                throw new NullReferenceException();
            }
            user.Status = (int)newStatus;

            await _dbContext.SaveChangesAsync();
        }

        public override async Task<List<ChangeSetModel>> AddAsync(UserDto dto)
        {
            dto.Rate = 1;

            var user = SqlUser.FromDto(dto);
            await _dbContext.Users.AddAsync(user);

            var changeSet = CreateChangeSetEntity(Operation.Insert, dto.Id);
            await _dbContext.ChangeSets.AddAsync(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public override async Task<List<ChangeSetModel>> UpdateAsync(UserDto dto)
        {
            var existingEntity = await GetAsync(dto.Id);
            var existingDto = UserDto.FromModel(existingEntity);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }

            var user = SqlUser.FromDto(dto);
            _dbContext.Users.Update(user);

            var changeSet = CreateChangeSetEntity(Operation.Update, dto.Id, changes);
            _dbContext.ChangeSets.Add(changeSet);

            _dbContext.SaveChanges();
            return new List<ChangeSetModel> { ChangeSetModel.FromEntity(changeSet) };
        }

        public async Task<UserModel?> FindByEmail(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
            return user == null ? null : UserModel.FromEntity(user);
        }
        public async Task RegisterAsync(UserDto newUserDto)
        {
            var user = new SqlUser
            {
                Email = newUserDto.Email,
                UserName = newUserDto.Email,
                FirstName = newUserDto.FirstName,
                LastName = newUserDto.LastName,
                Patronymic = newUserDto.Patronymic,
            };

            var confirmationTokenExpiration = DateTime.UtcNow.AddDays(30);
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmEmailLink = new Uri(QueryHelpers.AddQueryString($"{Constants.ProdFrontHost}/login",
                new Dictionary<string, string?>() { { "token", confirmationToken }, })).ToString();

            var message = new EmailMessage(new string[] { newUserDto.Email! },
                _localizer["Email:Confirm_email_subject"],
                _localizer["Email:Confirm_email_html", confirmEmailLink]);

            _emailService.Send(message);

            var result = await _userManager.CreateAsync(user, newUserDto.Password);
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
        private string GenerateToken(string userId, string signingKeyAsText)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKeyAsText));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                 new Claim(ClaimTypes.Name, userId.ToString())
                    // Add other claims as needed
                }),
                Expires = DateTime.UtcNow.AddDays(7), // Token expiration, adjust as needed
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
        public async Task<string> SignInAsync(string email, string signingKeyAsText)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
            if (user == null)
            {
                throw new NullReferenceException("User not found");
            }

            // Sign in the user
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Generate JWT token
            var accessToken = GenerateToken(user.Id, signingKeyAsText);
            return accessToken;
        }
        public async Task<string> SignInAsync(string email, string password, string signingKeyAsText)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.Equals(email));
            if (user == null)
            {
                throw new NullReferenceException("User not found");
            }

            // Sign in the user
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid credentials");
            }

            // Generate JWT token
            var accessToken = GenerateToken(user.Id, signingKeyAsText);
            return accessToken;
        }
        public async Task<bool> VerifyAsync(string userId, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new NullReferenceException();
            }

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        public override async Task<List<UserModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlUser>().Select(e => e.Id).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlUser>()
              .Where(e => randomlySelectedIds.Contains(e.Id))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(UserModel.FromEntity).ToList();
            return models;
        }

        public async Task<UserModel> GetByEmailAsync(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException();
            }

            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email!.ToLower().Equals(email.ToLower()));
            return UserModel.FromEntity(user);
        }

        public override async Task<UserModel> GetAsync(string entityId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id!.Equals(entityId));
            return UserModel.FromEntity(user);
        }

        public override async Task<List<UserModel>> TakeAsync(int offset, int limit)
        {
            var entities = await _dbContext.Users
                .Skip(offset)
                .Take(limit)
                .Cast<SqlUser>()
                .ToListAsync();

            return entities.Select(UserModel.FromEntity).ToList();
        }


    }
}
