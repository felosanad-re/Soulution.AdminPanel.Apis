using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.ResponseDTO.User;
using AdminPanel.Core.ModelsDTO.RequestDTO.Creation;
using AdminPanel.Core.ModelsDTO.RequestDTO.Register;
using AdminPanel.Core.ModelsDTO.ResponseDTO;
using AdminPanel.Core.Service_Contract.UserServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminPanel.Apis.Controllers.Users
{
    public class UserController : BaseController
    {
        #region Services
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        #endregion

        #region Create User
        [HttpPost("CreateUser")] // Post: /api/User/CreateUser
        public async Task<ActionResult<ResultServiceApplication<CreateToReturnDTO>>> Create([FromBody] CreateDTO register)
        {
            var result = await _userService.CreateAccountAsync(register);
            if (!result.Succeed) return BadRequest(result.Errors);
            return Ok(result);
        }
        #endregion

        #region set Password
        [HttpPost("setPassword")] // Get: /api/User/setPassword
        public async Task<ActionResult<ResultServiceApplication<ApplicationUserToReturnDTO>>> SetAdminPassword([FromBody] SetAdminPasswordDTO request)
        {
            var result = await _userService.SetPasswordAsync(request);
            if (!result.Succeed) return BadRequest(result.Errors);
            return Ok(result);
        }
        #endregion

        #region Get User
        [Authorize]
        [HttpGet("currentUser")]
        public async Task<ActionResult<ResultServiceApplication<ApplicationUserToReturnDTO>>> GetUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();
            var user = await _userService.GetCurrentUser(userId);
            if (user is null) return NotFound(user.Errors);
            return Ok(user);
        }
        #endregion
    }
}
