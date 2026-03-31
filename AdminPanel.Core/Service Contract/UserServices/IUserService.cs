using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.ResponseDTO.User;
using AdminPanel.Core.ModelsDTO.RequestDTO.Creation;
using AdminPanel.Core.ModelsDTO.RequestDTO.Register;
using AdminPanel.Core.ModelsDTO.ResponseDTO;

namespace AdminPanel.Core.Service_Contract.UserServices
{
    public interface IUserService
    {
        Task<ResultServiceApplication<CreateToReturnDTO>> CreateAccountAsync(CreateDTO dTO);
        Task<ResultServiceApplication<ApplicationUserToReturnDTO>> SetPasswordAsync(SetAdminPasswordDTO dTO);
        Task<ResultServiceApplication<ApplicationUserToReturnDTO>> GetCurrentUser(string userId);
    }
}
