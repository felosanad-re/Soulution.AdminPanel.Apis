using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.ResponseDTO.User;
using AdminPanel.Core.ModelsDTO.RequestDTO.Creation;
using AdminPanel.Core.ModelsDTO.RequestDTO.Register;
using AdminPanel.Core.ModelsDTO.ResponseDTO;
using AdminPanel.Core.Service_Contract.AuthServices;
using AdminPanel.Core.Service_Contract.UserServices;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace AdminPanel.Services.UserServices
{
    public class UserService : IUserService
    {
        #region Services
        protected readonly IAuthService _authService;
        protected readonly UserManager<ApplicationUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IMapper _mapper;
        protected readonly IEmailSender _emailSender;
        protected readonly IConfiguration _configuration;
        public UserService(IAuthService authService, UserManager<ApplicationUser> userManager, IMapper mapper, IEmailSender emailSender, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _authService = authService;
            _userManager = userManager;
            _mapper = mapper;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _configuration = configuration;
        }
        #endregion

        #region Create Account Async
        public async Task<ResultServiceApplication<CreateToReturnDTO>> CreateAccountAsync(CreateDTO dTO)
        {
            // if Email Is Exist
            if (await _userManager.FindByEmailAsync(dTO.Email) != null)
                return ResultServiceApplication<CreateToReturnDTO>.Fail("This User Is Already Exist");

            // Check on role
            if (!await _roleManager.RoleExistsAsync(dTO.Role)) return ResultServiceApplication<CreateToReturnDTO>.Fail("Roles Not Found");

            var user = new ApplicationUser()
            {
                UserName = dTO.UserName,
                FirstName = dTO.FirstName,
                LastName = dTO.LastName,
                Email = dTO.Email,
                Address = dTO.Address,
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return ResultServiceApplication<CreateToReturnDTO>.Fail(string.Join(", ", result.Errors.Select(e => e.Description)));

            var roleResult = await _userManager.AddToRoleAsync(user, dTO.Role);
            if (!roleResult.Succeeded) return ResultServiceApplication<CreateToReturnDTO>.Fail("Roles Not Set");

            // Send Email
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);
            var link = $"{_configuration["FrontEndUrl"]}/dashboard/setPassword?userId={user.Id}&token={token}"; // Change Link To Project Angular
            // https://localhost:4200/setPassword?userId={user.Id}&token={token}
            await _emailSender.SendEmailAsync(
                user.Email,
                "Set your password",
                $"<h1>Click <a href='{link}'>here</a> to set your password</h1>"
            );
            var data = _mapper.Map<CreateToReturnDTO>(user);
            return ResultServiceApplication<CreateToReturnDTO>.Success(data, "User created successfully. Email sent to set password.");
        }

        #endregion

        #region Set Admin Password
        public async Task<ResultServiceApplication<ApplicationUserToReturnDTO>> SetPasswordAsync(SetAdminPasswordDTO dTO)
        {
            var user = await _userManager.FindByIdAsync(dTO.UserId);
            // Check User
            if (user is null) return ResultServiceApplication<ApplicationUserToReturnDTO>.Fail("User Not Found");

            var res = await _userManager.ResetPasswordAsync(user, dTO.Token, dTO.Password);
            if (!res.Succeeded) return ResultServiceApplication<ApplicationUserToReturnDTO>.Fail(string.Join(", ", res.Errors.Select(e => e.Description)));
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            var data = _mapper.Map<ApplicationUserToReturnDTO>(user);
            return ResultServiceApplication<ApplicationUserToReturnDTO>.Success(data, "Password Set Succeeded & Account Confirmed");
        }
        #endregion

        public async Task<ResultServiceApplication<ApplicationUserToReturnDTO>> GetCurrentUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if(user is null) return ResultServiceApplication<ApplicationUserToReturnDTO>.Fail("User Not Found");
            var data = _mapper.Map<ApplicationUserToReturnDTO>(user);
            return ResultServiceApplication<ApplicationUserToReturnDTO>.Success(data,"This user details");
        }
    }
}
