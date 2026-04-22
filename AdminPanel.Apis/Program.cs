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
            try
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
            catch (Exception ex)
            {
                WriteStartupFailure(ex);
                throw;
            }
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

            if (string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
            {
                var passwordFromFallbackProvider = TryGetPasswordFromFallbackConnectionString(configuration);
                if (!string.IsNullOrWhiteSpace(passwordFromFallbackProvider))
                {
                    connectionStringBuilder.Password = passwordFromFallbackProvider;
                }
            }

            if (string.IsNullOrWhiteSpace(connectionStringBuilder.Password))
            {
                var passwordFromProductionFile = TryGetPasswordFromJsonFile(configuration, "appsettings.Production.json");
                if (!string.IsNullOrWhiteSpace(passwordFromProductionFile))
                {
                    connectionStringBuilder.Password = passwordFromProductionFile;
                }
            }

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

        private static string? TryGetPasswordFromFallbackConnectionString(IConfiguration configuration)
        {
            if (configuration is not IConfigurationRoot configurationRoot)
            {
                return null;
            }

            foreach (var provider in configurationRoot.Providers)
            {
                if (!provider.TryGet("ConnectionStrings:Default", out var candidateConnectionString) ||
                    string.IsNullOrWhiteSpace(candidateConnectionString))
                {
                    continue;
                }

                try
                {
                    var candidateBuilder = new SqlConnectionStringBuilder(candidateConnectionString);
                    if (!string.IsNullOrWhiteSpace(candidateBuilder.Password))
                    {
                        return candidateBuilder.Password;
                    }
                }
                catch (ArgumentException)
                {
                    // Ignore malformed connection strings from optional providers and keep searching.
                }
            }

            return null;
        }

        private static string? TryGetPasswordFromJsonFile(IConfiguration configuration, string fileName)
        {
            var contentRoot = configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
            if (string.IsNullOrWhiteSpace(contentRoot))
            {
                contentRoot = AppContext.BaseDirectory;
            }

            var fullPath = Path.Combine(contentRoot, fileName);
            if (!File.Exists(fullPath))
            {
                return null;
            }

            try
            {
                var directConfiguration = new ConfigurationBuilder()
                    .SetBasePath(contentRoot)
                    .AddJsonFile(fileName, optional: false, reloadOnChange: false)
                    .Build();

                var candidateConnectionString = directConfiguration.GetConnectionString("Default");
                if (string.IsNullOrWhiteSpace(candidateConnectionString))
                {
                    return null;
                }

                var candidateBuilder = new SqlConnectionStringBuilder(candidateConnectionString);
                return string.IsNullOrWhiteSpace(candidateBuilder.Password) ? null : candidateBuilder.Password;
            }
            catch
            {
                return null;
            }
        }

        // Write Error in log file
        private static void WriteStartupFailure(Exception ex)
        {
            var message = $"""
[{DateTime.UtcNow:O}] Application startup failed.
{FlattenException(ex)}

""";

            foreach (var logPath in GetStartupLogPaths())
            {
                try
                {
                    var directory = Path.GetDirectoryName(logPath);
                    if (!string.IsNullOrWhiteSpace(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    File.AppendAllText(logPath, message);
                }
                catch
                {
                    // Try the next writable location.
                }
            }

            try
            {
                Console.Error.WriteLine(message);
            }
            catch
            {
                Console.Error.WriteLine(ex.ToString());
            }
        }

        private static IEnumerable<string> GetStartupLogPaths()
        {
            var paths = new[]
            {
                Path.Combine(AppContext.BaseDirectory, "startup-error.log"),
                Path.Combine(AppContext.BaseDirectory, "logs", "startup-error.log"),
                Path.Combine(Path.GetTempPath(), "AdminPanel.Apis", "startup-error.log")
            };

            return paths.Distinct(StringComparer.OrdinalIgnoreCase);
        }

        private static string FlattenException(Exception ex)
        {
            var lines = new List<string>();
            var current = ex;
            var level = 0;

            while (current != null)
            {
                lines.Add($"Level {level}: {current.GetType().FullName}");
                lines.Add($"Message: {current.Message}");
                lines.Add(current.StackTrace ?? "No stack trace available.");
                lines.Add(string.Empty);
                current = current.InnerException!;
                level++;
            }

            return string.Join(Environment.NewLine, lines);
        }
    }
}
