using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Roles;
using AdminPanel.Core.ModelsDto.ResponseDTO.Roles;
using AdminPanel.Core.Service_Contract.RolesServices;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Services.RolesServices
{
    public class RoleService : IRoleService
    {
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected readonly IMapper _mapper;

        public RoleService(RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }

        #region Get All
        public ResultServiceApplication<IReadOnlyList<RoleToReturnDTO>> GetAll()
        {
            var roles = _roleManager.Roles.ToList();
            if (!roles.Any()) return ResultServiceApplication<IReadOnlyList<RoleToReturnDTO>>.Fail("There Is No Roles");
            var data = _mapper.Map<IReadOnlyList<RoleToReturnDTO>>(roles);

            return ResultServiceApplication<IReadOnlyList<RoleToReturnDTO>>.Success(data, "Roles retrieved successfully");
        }
        #endregion

        #region Get Role
        public async Task<ResultServiceApplication<RoleToReturnDTO>> GetAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return ResultServiceApplication<RoleToReturnDTO>.Fail("Role Not Found");
            var data = _mapper.Map<RoleToReturnDTO>(role);
            return ResultServiceApplication<RoleToReturnDTO>.Success(data, "Role Was Exist");
        }
        #endregion

        #region Add Role Async
        public async Task<ResultServiceApplication<RoleToReturnDTO>> AddRoleAsync(CreatedRoleDTO dTO)
        {
            var role = new IdentityRole()
            {
                Name = dTO.Name
            };

            if(await _roleManager.RoleExistsAsync(dTO.Name)) return ResultServiceApplication<RoleToReturnDTO>.Fail("this role is already exist");
            var result = await _roleManager.CreateAsync(role);
            if(!result.Succeeded) return ResultServiceApplication<RoleToReturnDTO>.Fail("role not Created");
            var data = _mapper.Map<RoleToReturnDTO>(role);
            return ResultServiceApplication<RoleToReturnDTO>.Success(data, "role created successfully");
        }
        #endregion

        #region Update Role Async
        public async Task<ResultServiceApplication<RoleToReturnDTO>> UpdateRoleAsync(UpdatedRoleDTO dTO)
        {
            var role = await _roleManager.FindByIdAsync(dTO.RoleId);
            if(role is null) return ResultServiceApplication<RoleToReturnDTO>.Fail("role not found");

            role.Name = dTO.Name;
            // check
            if(await _roleManager.RoleExistsAsync(dTO.Name) && role.Name != dTO.Name) return ResultServiceApplication<RoleToReturnDTO>.Fail("this role is already exist");

            var result = await _roleManager.UpdateAsync(role);
            if(!result.Succeeded) return ResultServiceApplication<RoleToReturnDTO>.Fail("role not Updated");
            var data = _mapper.Map<RoleToReturnDTO>(role);
            return ResultServiceApplication<RoleToReturnDTO>.Success(data, "role updated successfully");
        }
        #endregion

        #region Delete Async
        public async Task<ResultServiceApplication<bool>> DeleteAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role is null) return ResultServiceApplication<bool>.Fail("role not found");
            var result = await _roleManager.DeleteAsync(role);
            if(!result.Succeeded) return ResultServiceApplication<bool>.Fail("role not deleted");
            return ResultServiceApplication<bool>.Success(true, "role deleted successfully");
        }
        #endregion
    }
}
