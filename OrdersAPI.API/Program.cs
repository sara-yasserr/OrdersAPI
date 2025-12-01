using Microsoft.EntityFrameworkCore;
using OrdersAPI.Core.Interfaces;
using OrdersAPI.Infrastructure.Data;
using Serilog;

namespace OrdersAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Serilog
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/orderapi-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            builder.Host.UseSerilog();
            #endregion

            // Add services to the container.

            builder.Services.AddControllers();

            #region Configure EF with SQL Server
            // Configure EF Core with SQL Server
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            #endregion

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseAuthorization();


            app.MapControllers();

            try
            {
                Log.Information("Starting OrdersAPI application");
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
