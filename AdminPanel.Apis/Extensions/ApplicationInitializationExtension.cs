using AdminPanel.Core.Service_Contract;
using AdminPanel.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdminPanel.Apis.Extensions
{
    public static class ApplicationInitializationExtension
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            // Add Scope
            using var scope = app.Services.CreateScope();
            // Add Services
            var services = scope.ServiceProvider;
            // Create object From DbContext Implicitly
            var _context = services.GetRequiredService<AdminDbContext>();
            // Create object From DbInitialize Implicitly
            var _dbInitialization = services.GetRequiredService<IDbInitialize>();
            var logger = services.GetRequiredService<ILoggerFactory>();

            try
            {
                await _context.Database.MigrateAsync();
                await _dbInitialization.CreateInitializationAsync();
            }
            catch (Exception ex)
            {
                var _logger = logger.CreateLogger<Program>();
                _logger.LogError(ex, "Error in database");
            }
        }
    }
}
