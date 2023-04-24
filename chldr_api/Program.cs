using chldr_tools;

namespace chldr_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<SqlContext>();
            builder.Services.AddGraphQLServer()
                .AddQueryType<Query>()
                .AddProjections()
                .AddFiltering()
                .AddSorting();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
             
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();
            app.MapGraphQL("/graphql");
            app.Run();
        }
    }
}