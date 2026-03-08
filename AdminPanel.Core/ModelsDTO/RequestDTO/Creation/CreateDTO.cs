using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDTO.RequestDTO.Register
{
    public class CreateDTO
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; }
        public string? Address { get; set; }
    }
}
