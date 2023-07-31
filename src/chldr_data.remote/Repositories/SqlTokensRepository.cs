using chldr_data.DatabaseObjects.Dtos;
using chldr_data.DatabaseObjects.Models;
using chldr_data.Enums;
using chldr_data.Interfaces;
using chldr_data.Interfaces.Repositories;
using chldr_data.Models;
using chldr_data.remote.Interfaces;
using chldr_data.remote.Services;
using chldr_data.remote.SqlEntities;
using chldr_utils.Services;
using GraphQLParser;
using Microsoft.EntityFrameworkCore;

namespace chldr_data.remote.Repositories
{
    public class SqlTokensRepository : SqlRepository<SqlToken, TokenModel, TokenDto>, ITokensRepository
    {
        public SqlTokensRepository(DbContextOptions<SqlContext> dbConfig, FileService fileService, string userId) : base(dbConfig, fileService, userId) { }

        protected override RecordType RecordType => RecordType.Token;

        public override async Task<List<TokenModel>> GetRandomsAsync(int limit)
        {
            var randomizer = new Random();
            var ids = await _dbContext.Set<SqlToken>().Select(e => e.TokenId).ToListAsync();
            var randomlySelectedIds = ids.OrderBy(x => randomizer.Next(1, Constants.EntriesApproximateCoount)).Take(limit).ToList();

            var entities = await _dbContext.Set<SqlToken>()
              .Where(e => randomlySelectedIds.Contains(e.TokenId))
              .AsNoTracking()
              .ToListAsync();

            var models = entities.Select(FromEntityShortcut).ToList();
            return models;
        }
        public override async Task<List<ChangeSetModel>> Add(TokenDto dto)
        {
            var token = SqlToken.FromDto(dto);
            _dbContext.Add(token);
            _dbContext.SaveChanges();

            return new List<ChangeSetModel>();
        }

        public async Task<TokenModel?> FindByValueAsync(string tokenValue)
        {
            var token = await _dbContext.Tokens.SingleOrDefaultAsync(t => t.Value.Equals(tokenValue));
            if (token == null)
            {
                return null;
            }

            return TokenModel.FromEntity(token);
        }

        public IEnumerable<TokenModel> GetByUserId(string userId, TokenType type, bool ignoreExpired = true)
        {
            var tokens = _dbContext.Tokens
                    .Where(t => t.Type == (int)type)
                    .Where(t => t.UserId.Equals(userId));

            if (ignoreExpired)
            {
                tokens.Where(t => t.ExpiresIn > DateTimeOffset.UtcNow);
            }

            return tokens.Select(t => TokenModel.FromEntity(t));
        }

        public async Task<TokenModel?> GetByValueAsync(string refreshToken)
        {
            var tokenInDatabase = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.Value == refreshToken);
            return tokenInDatabase == null ? null : TokenModel.FromEntity(tokenInDatabase);
        }

        public async Task<TokenModel?> GetPasswordResetTokenAsync(string tokenValue)
        {
            var tokenInDatabase = await _dbContext.Tokens.FirstOrDefaultAsync(t => t.Type == (int)TokenType.PasswordReset && t.Value == tokenValue && t.ExpiresIn > DateTimeOffset.UtcNow);
            return tokenInDatabase == null ? null : TokenModel.FromEntity(tokenInDatabase);
        }

        public override async Task<List<ChangeSetModel>> Update(TokenDto dto)
        {
            var existing = await GetAsync(dto.TokenId);
            var existingDto = TokenDto.FromModel(existing);

            var changes = Change.GetChanges(dto, existingDto);
            if (changes.Count == 0)
            {
                return new List<ChangeSetModel>();
            }
            ApplyChanges<SqlSound>(dto.TokenId, changes);

            _dbContext.SaveChanges();

            return new List<ChangeSetModel>();
        }

        protected override TokenModel FromEntityShortcut(SqlToken entity)
        {
            return TokenModel.FromEntity(entity);
        }
    }
}
