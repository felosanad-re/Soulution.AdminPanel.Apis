using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Roles;
using AdminPanel.Core.ModelsDto.ResponseDTO.Roles;

namespace AdminPanel.Core.Service_Contract.RolesServices
{
    public interface IRoleService
    {
        ResultServiceApplication<IReadOnlyList<RoleToReturnDTO>> GetAll();
        Task<ResultServiceApplication<RoleToReturnDTO>> GetAsync(string id);
        Task<ResultServiceApplication<RoleToReturnDTO>> AddRoleAsync(CreatedRoleDTO dTO);
        Task<ResultServiceApplication<RoleToReturnDTO>> UpdateRoleAsync(UpdatedRoleDTO dTO);
        Task<ResultServiceApplication<bool>> DeleteAsync(string id);
    }
}
