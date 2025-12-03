using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using OrdersAPI.API.Middleware;
using OrdersAPI.Core.Interfaces;
using OrdersAPI.Infrastructure.Data;
using OrdersAPI.Infrastructure.Repositories;
using OrdersAPI.Infrastructure.Services;
using AutoMapper;
//using AutoMapper.Extensions.Microsoft.DependencyInjection

using Serilog;
using OrdersAPI.API.Mappings;

namespace OrdersAPI.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .WriteTo.Console()
                .WriteTo.File("logs/orderapi-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            builder.Host.UseSerilog();
            #endregion

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(cfg =>{cfg.AddProfile(new AutoMapperConfig());});

            #region Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });
            #endregion

            #region Configure Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()));
            #endregion

            #region Configure Redis
            builder.Services.AddSingleton<ICacheService>(sp =>
            {
                var configuration = builder.Configuration.GetSection("Redis:Configuration").Value
                    ?? throw new InvalidOperationException("Redis configuration is missing");
                return new RedisCacheService(configuration);
            });
            #endregion

            #region Register Services
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            #endregion

            #region Configure Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Orders API",
                    Version = "v1",
                    Description = "A RESTful API for managing orders with Redis caching",
                    Contact = new OpenApiContact
                    {
                        Name = "Sara Yasser",
                        Email = "sarahyasser979@gmail.com"
                    }
                });
            });
            #endregion

            var app = builder.Build();

            #region Configure Middleware Pipeline
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Orders API V1");
                    options.RoutePrefix = string.Empty;
                    options.DocumentTitle = "Orders API Documentation";
                });
            }

            app.UseCors("AllowAll");

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();
            #endregion

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