using AdminPanel.Apis.Errors_Handler;
using AdminPanel.Core.ModelsDto.RequestDTO.ForgetPassword;
using AdminPanel.Core.ModelsDTO.RequestDTO.Login;
using AdminPanel.Core.ModelsDTO.ResponseDTO.Login;
using AdminPanel.Core.Service_Contract.AuthServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Accounts
{
    public class AccountController : BaseController
    {
        #region Services
        private readonly IAuthService _authService;

        public AccountController( IAuthService authService)
        {
            _authService = authService;
        }
        #endregion

        #region Login
        [HttpPost("LogIn")] // Post: /api/Account/LogIn
        [ProducesResponseType(typeof(LoginToReturnDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<LoginToReturnDTO>> Login([FromBody] LoginDTO request)
        {
            var user = await _authService.LoginAsync(request);

            return Ok(user);
        }
        #endregion

        [HttpPost("ForgetPassword")]
        public async Task<ActionResult> ForgetPassword([FromBody] ForgetPasswordDTO request)
        {
            var user = await _authService.ForgetPasswordAsync(request);
            if (user == null) return BadRequest(new {Message = "Invalid Email"});

            return Ok(user);
        }

        // In Angular will received Token And userId In Query Params and send them in body
        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody]ResetPasswordDTO request)
        {
            var user = await _authService.ResetPasswordAsync(request);
            if (user == null) return BadRequest(new { Message = "Invalid User" });
            return Ok(user);
        }
    }
}
