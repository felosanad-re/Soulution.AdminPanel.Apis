using AdminPanel.Apis.Errors_Handler;
using AdminPanel.Apis.Extensions;
using AdminPanel.Core.Entities.Identity;
using AdminPanel.Repositories.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AdminPanel.Apis
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Add DI Services
            // Add services to the container.

            builder.Services.AddControllers()
                .AddViewLocalization() // Localization
                .AddDataAnnotationsLocalization();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();

            // Add Connection String
            builder.Services.AddDbContext<AdminDbContext>(options =>
            {
                options.UseSqlServer(BuildConnectionString(builder.Configuration));
            });

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 5;
            }).AddEntityFrameworkStores<AdminDbContext>().AddDefaultTokenProviders();

            // Add DI Services For Application
            builder.Services.AddApplicationServices(builder.Configuration);

            builder.Services.AddErrorMessage();

            var allowedOrigins = builder.Configuration.GetSection("AllowCORS").Get<string[]>();
            //Add Policy
            builder.Services.AddCors(action =>
            {
                action.AddPolicy("Angular", policy =>
                {
                    policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    //.AllowCredentials();
                });
            });
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
            app.UseStaticFiles();
            app.UseRouting();

            // Use Localization File
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en"),
                SupportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en") },
                SupportedUICultures = new[] {new CultureInfo("ar"), new CultureInfo("en")}
            });
            app.UseCors("Angular");
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            #endregion

            app.Run();
        }

        private static string BuildConnectionString(IConfiguration configuration)
        {
            var configuredConnectionString = configuration.GetConnectionString("Default");
            if (string.IsNullOrWhiteSpace(configuredConnectionString))
            {
                throw new InvalidOperationException("Connection string 'Default' is missing.");
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(configuredConnectionString);
            var passwordFromSecret = configuration["ConnectionStrings:DefaultPassword"];

            if (!string.IsNullOrWhiteSpace(passwordFromSecret))
            {
                connectionStringBuilder.Password = passwordFromSecret;
            }

            if (!connectionStringBuilder.IntegratedSecurity && string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
            {
                throw new InvalidOperationException("Database password is missing. Set 'ConnectionStrings__DefaultPassword' or provide a full 'ConnectionStrings__Default' value outside git.");
            }

            return connectionStringBuilder.ConnectionString;
        }
    }
}
