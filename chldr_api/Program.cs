using chldr_api.GraphQL.MutationServices;
using chldr_api.GraphQL.ServiceResolvers;
using chldr_data.Repositories;
using chldr_tools;
using chldr_utils.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection.PortableExecutable;
using System.Text;

namespace chldr_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddDbContext<SqlContext>(options =>
                options.UseMySQL("server=localhost;port=3306;database=xj_chldr;user=root;password=2398ehh&*H!&*Hhs-="),
                ServiceLifetime.Singleton);

            builder.Services.AddScoped<UpdateWordResolver>();
        
            builder.Services.AddScoped<PasswordResetResolver>();
            builder.Services.AddScoped<UpdatePasswordResolver>();
            builder.Services.AddScoped<RegisterUserResolver>();
            builder.Services.AddScoped<ConfirmEmailResolver>();
            builder.Services.AddScoped<LoginResolver>();

            // Repositories
            //appBuilder.Services.AddSingleton<WordsRepository>();
            //appBuilder.Services.AddSingleton<LanguagesRepository>();
            //appBuilder.Services.AddSingleton<PhrasesRepository>();
            ////appBuilder.Services.AddSingleton<SourcesRepository>();
            //appBuilder.Services.AddSingleton<UsersRepository>();

            builder.Services.AddSingleton<EmailService>();
            builder.Services.AddLocalization();

            var signingKey = new SymmetricSecurityKey(
         Encoding.UTF8.GetBytes("MySuperSecretKey"));

            builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidIssuer = "https://auth.chillicream.com",
                        ValidAudience = "https://graphql.chillicream.com",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey
                    };
            });

            builder.Services.AddAuthorization();

            builder.Services.AddGraphQLServer()
                .AddQueryType<Query>()
                .AddMutationType<Mutation>()
                .AddProjections()
                .AddFiltering()
                .AddSorting();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGraphQL("/graphql");
            app.Run();
        }
    }
}