using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Roles;
using AdminPanel.Core.ModelsDto.ResponseDTO.Roles;
using AdminPanel.Core.Service_Contract.RolesServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Roles
{
    public class RoleController : BaseController
    {
        protected readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        #region Get All
        [HttpGet("Roles")] // GEt: /api/Role/Roles
        public ActionResult<ResultServiceApplication<IReadOnlyList<RoleToReturnDTO>>> GetAll()
        {
            var result = _roleService.GetAll();
            if (!result.Succeed) return BadRequest(result.Errors);
            return Ok(result);
        }
        #endregion

        #region Get Role
        [HttpGet("Role/{id}")] // Get: /api/role/Role/id
        public async Task<ActionResult<ResultServiceApplication<RoleToReturnDTO>>> GetRole(string id)
        {
            var result = await _roleService.GetAsync(id);
            if(!result.Succeed) return BadRequest(result.Errors);
            return Ok(result);
        }
        #endregion

        #region Add Role
        [HttpPost("AddRole")] // Post: /api/role/AddRole
        public async Task<ActionResult<ResultServiceApplication<RoleToReturnDTO>>> AddRole([FromBody]CreatedRoleDTO dTO)
        {
            var result = await _roleService.AddRoleAsync(dTO);
            if(!result.Succeed) return BadRequest(result);
            return Ok(result);
        }
        #endregion

        #region Update Role
        [HttpPut("UpdateRole")] // put: /api/role/UpdateRole
        public async Task<ActionResult<ResultServiceApplication<RoleToReturnDTO>>> UpdateRole([FromBody] UpdatedRoleDTO dTO)
        {
            var result = await _roleService.UpdateRoleAsync(dTO);
            if (!result.Succeed) return BadRequest(result);
            return Ok(result);
        }
        #endregion

        #region Delete Role
        [HttpDelete("DeleteRole/{id}")] //Delete: /api/role/DeleteRole
        public async Task<ActionResult<ResultServiceApplication<bool>>> DeleteRole(string id)
        {
            var result = await _roleService.DeleteAsync(id);
            if (!result.Succeed) return BadRequest(result);
            return Ok(result);
        }
        #endregion

    }
}
