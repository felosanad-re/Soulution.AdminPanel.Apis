using AdminPanel.Core;
using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDTO.RequestDTO.Creation;
using AdminPanel.Core.ModelsDTO.RequestDTO.Register;
using AdminPanel.Core.ModelsDTO.ResponseDTO;
using AdminPanel.Core.Service_Contract.AuthServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdminPanel.Apis.Controllers.Users
{
    public class UserController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IAuthService _authService;
        public UserController(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IAuthService authService)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _authService = authService;
        }
        #region CreateUser
        [HttpPost("CreateUser")] // Post: /api/Account/CreateUser
        public async Task<ActionResult<CreateToReturnDTO>> Create([FromBody] CreateDTO register)
        {
            // if Email Is Exist
            if (await _userManager.FindByEmailAsync(register.Email) != null)
                return BadRequest(new { Message = "Email already exists" });

            var user = new ApplicationUser()
            {
                UserName = register.UserName,
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                Address = register.Address,
            };
            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Add Role
            var roleResult = await _userManager.AddToRoleAsync(user, Roles.User);
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            // Send Email
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebUtility.UrlEncode(token);
            var link = $"https://localhost:4200/set-password?userId={user.Id}&token={token}"; // Change Link To Project Angular
            // https://localhost:4200/setPassword?userId={user.Id}&token={token}
            await _emailSender.SendEmailAsync(
                user.Email,
                "Set your password",
                $"<h1>Click <a href='{link}'>here</a> to set your password</h1>"
            );

            return Ok(new { Message = "User created successfully. Email sent to set password." });
        }
        #endregion

        #region setPassword
        [HttpPost("setPassword")] // Get: /api/Account/setPassword
        public async Task<ActionResult<ServiceResult>> SetAdminPassword([FromBody] SetAdminPasswordDTO request)
        {
            var result = await _authService.SetAdminPasswordAsync(request);
            if (!result.Succeed)
                return BadRequest(result);
            return Ok(result);
        }
        #endregion
    }
}
