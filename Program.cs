using Microsoft.EntityFrameworkCore;
using ZeissJwtDemo.Context;

namespace ZeissJwtDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AuthProjectContext>(options =>
            {
                options.UseSqlite("Data Source=auth.db");
            });

            var app = builder.Build();

            app.MapControllers();

            app.Run();
        }
    }
}
