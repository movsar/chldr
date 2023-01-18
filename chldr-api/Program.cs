using chldr_data.Services;

namespace user_management
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddSingleton<DataAccess>();

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            app.UseAuthorization();

            app.MapControllers();
            app.MapGet("/", () => { return "You shouldn't be here"; });

            app.Run();
        }
    }
}