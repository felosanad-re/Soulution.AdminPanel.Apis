using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.ForgetPassword;
using AdminPanel.Core.ModelsDTO.RequestDTO.Creation;
using AdminPanel.Core.ModelsDTO.RequestDTO.Login;
using AdminPanel.Core.ModelsDTO.ResponseDTO.Login;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Core.Service_Contract.AuthServices
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
        Task<LoginToReturnDTO> LoginAsync(LoginDTO login);
        Task<ServiceResult> ForgetPasswordAsync(string email);
        Task<ServiceResult> SetAdminPasswordAsync(SetAdminPasswordDTO request);
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDTO request);
    }
}
