using AdminPanel.Apis.Errors_Handler;
using AdminPanel.Apis.Extensions;
using AdminPanel.Core.Entities.Identity;
using AdminPanel.Repositories.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AdminPanel.Apis
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add DI Services
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            // Add Connection String
            builder.Services.AddDbContext<AdminDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
            });

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
            }).AddEntityFrameworkStores<AdminDbContext>().AddDefaultTokenProviders();

            // Add DI Services For Application
            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddErrorMessage();
            #endregion

            var app = builder.Build();

            // Database Initialize
            await app.InitializeDatabaseAsync();

            app.UseMiddleware<ExceptionMiddleware>(); // Global Error Handler
            #region Add Configurations MidealWears
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }
    }
}
