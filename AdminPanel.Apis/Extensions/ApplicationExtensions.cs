using AdminPanel.Apis.Helpers;
using AdminPanel.Apis.Helpers.Mapping;
using AdminPanel.Core.Service_Contract;
using AdminPanel.Core.Service_Contract.AttachmentServices;
using AdminPanel.Core.Service_Contract.AuthServices;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.UnitOfWork;
using AdminPanel.Repositories.UnitOfWorks;
using AdminPanel.Services;
using AdminPanel.Services.AttachmentServices;
using AdminPanel.Services.AuthServices;
using AdminPanel.Services.ProductServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AdminPanel.Apis.Extensions
{
    public static class ApplicationExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration _configuration)
        {
            // Add Profile Mapping
            services.AddAutoMapper(typeof(ProfileMapping));
            // Add Product Service
            services.AddScoped<IProductService, ProductService>();
            // Add Attachment Services
            services.AddScoped<IAttachmentService, AttachmentService>();
            // Add Unit Of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Add DbInitialization Services
            services.AddScoped(typeof(IDbInitialize), typeof(DbInitialization));
            // Add Auth Services
            services.AddScoped(typeof(IAuthService), typeof(AuthService));

            // Add Email Sender
            services.AddTransient<IEmailSender, EmailSender>();
            // Add JWT
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = true,
                    ValidAudience = _configuration["JWT:audience"],
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JWT:issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromDays(double.Parse(_configuration["JWT:expires"])),
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]))
                };
            });
            return services;
        }
    }
}
