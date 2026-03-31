using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDTO.RequestDTO.Creation
{
    public class SetAdminPasswordDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string Token { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "The confirm password not match with password")]
        public string ConfirmPassword { get; set; }
    }
}
