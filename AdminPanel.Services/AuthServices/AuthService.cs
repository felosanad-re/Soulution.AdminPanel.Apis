using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.ForgetPassword;
using AdminPanel.Core.ModelsDTO.RequestDTO.Creation;
using AdminPanel.Core.ModelsDTO.RequestDTO.Login;
using AdminPanel.Core.ModelsDTO.ResponseDTO.Login;
using AdminPanel.Core.Service_Contract.AuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace AdminPanel.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        #region Services
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager, IEmailSender emailSender, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }
        #endregion

        #region Create Token
        public async Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager){
            // Create Claims
            var CreateClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id!),
            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                CreateClaims.Add(new Claim(ClaimTypes.Role, role));

            // Create Key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));

            // Create Token Object
            var Token = new JwtSecurityToken(
                    issuer: _configuration["JWT:issuer"],
                    audience: _configuration["JWT:audience"],
                    CreateClaims,
                    expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:expires"]!)),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }
        #endregion

        #region Login
        public async Task<LoginToReturnDTO> LoginAsync(LoginDTO login)
        {
            var user = login.EmailOrUserName.Contains('@')?
                await _userManager.FindByEmailAsync(login.EmailOrUserName):
                await _userManager.FindByNameAsync(login.EmailOrUserName);

            if (user == null)
                return new LoginToReturnDTO()
                {
                    IsAuthenticated = false,
                    Message = "User not found"
                };

            var result = await _signInManager.PasswordSignInAsync(
                user,
                login.Password,
                login.RememberMe,
                lockoutOnFailure: true
            );

            if (result.IsLockedOut)
                return new LoginToReturnDTO() { IsAuthenticated = false, Message = "Your Account Is Locked" };

            if (result.IsNotAllowed)
                return new LoginToReturnDTO() { IsAuthenticated = false, Message = "Email Not Confirm" };

            if (!result.Succeeded)
                return new LoginToReturnDTO() { IsAuthenticated = false, Message = "Invalid Email Or Password" };
            var token = await CreateTokenAsync(user, _userManager);
            var roles = await _userManager.GetRolesAsync(user);
            return new LoginToReturnDTO()
            {
                IsAuthenticated = true,
                UserId = user.Id,
                Token = token,
                UserName = user.UserName,
                Roles = roles,
                Message = "Login Successful"
            };
        }
        #endregion

        #region Set Admin Password
        public async Task<ServiceResult> SetAdminPasswordAsync(SetAdminPasswordDTO request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            // Check User
            if (user is null) return new ServiceResult()
            {
                Succeed = false,
                Message = "User not found"
            };

            var res = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!res.Succeeded) return new ServiceResult()
            { 
                Succeed=false,
                Message = "Password Not Set",
                Errors = res.Errors.Select(e => e.Description)
            };
            return new ServiceResult()
            {
                Succeed = true,
                Message = "Password Set Successful"
            };
        }
        #endregion

        #region ForgetPasswordAsync
        public async Task<ServiceResult> ForgetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return new ServiceResult()
            {
                Message = "User not found",
                Succeed = false
            };
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);
            var link = $"https://localhost:4200/resetPassword?userId={user.Id}&token={token}";
            var message = $@"<h2>Password Reset</h2>
                <p>We received a request to reset your password.</p>
                <p>
                  <a href='{link}'>
                    Reset Password
                  </a>
                </p
                <p>If you didn't request this, ignore this email.</p>";

            await _emailSender.SendEmailAsync(user.Email, "Forget Password", message);
            return new ServiceResult()
            {
                Succeed = true,
                Message = "You Can Reset Your Password now"
            };
        }

        #endregion

        #region ResetPasswordAsync
        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return new ServiceResult()
            {
                Succeed = false,
                Message = "User Not Found"
            };

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!result.Succeeded) return new ServiceResult() { Succeed = false, Message = string.Join(",", result.Errors.Select(e => e.Description)) };

            return new ServiceResult()
            {
                Succeed = true,
                Message = "Password Changed Succeeded",
            };
        }
        #endregion

    }
}
