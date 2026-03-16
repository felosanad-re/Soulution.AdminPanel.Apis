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
        public string? Message { get; set; }
    }
}
