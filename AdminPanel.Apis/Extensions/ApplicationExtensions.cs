using AdminPanel.Apis.Helpers;
using AdminPanel.Apis.Helpers.Mapping;
using AdminPanel.Core.Service_Contract;
using AdminPanel.Core.Service_Contract.AttachmentServices;
using AdminPanel.Core.Service_Contract.AuthServices;
using AdminPanel.Core.Service_Contract.brandsServices;
using AdminPanel.Core.Service_Contract.CategoriesServices;
using AdminPanel.Core.Service_Contract.ChartsServices;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.Service_Contract.PurchaseServices;
using AdminPanel.Core.Service_Contract.ReportServices;
using AdminPanel.Core.Service_Contract.RolesServices;
using AdminPanel.Core.Service_Contract.UserServices;
using AdminPanel.Core.UnitOfWork;
using AdminPanel.Repositories.UnitOfWorks;
using AdminPanel.Services;
using AdminPanel.Services.AttachmentServices;
using AdminPanel.Services.AuthServices;
using AdminPanel.Services.BrandsServices;
using AdminPanel.Services.CategoriesServices;
using AdminPanel.Services.ChartServices;
using AdminPanel.Services.ProductServices;
using AdminPanel.Services.PurchaseServices;
using AdminPanel.Services.ReportTransactionServices;
using AdminPanel.Services.RolesServices;
using AdminPanel.Services.UserServices;
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
            // Add Chart Services
            services.AddScoped<IChartService, ChartService>();
            // Add Purchase Service
            services.AddScoped<IPurchaseService, PurchaseService>();
            // Add Role Service
            services.AddScoped<IRoleService, RoleService>();
            // Add Report Transaction Service
            services.AddScoped<IReportTransactionService, ReportTransactionService>();
            // Add Brand Service
            services.AddScoped<IBrandService, BrandService>();
            // Add Category Service
            services.AddScoped<ICategoryService, CategoryService>();
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
            // Add User Service
            services.AddScoped<IUserService, UserService>();
            // Add Role Services
            services.AddScoped<IRoleService, RoleService>();
            // Add Email Sender
            services.AddTransient<IEmailSender, EmailSender>();
            // Add JWT
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
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
