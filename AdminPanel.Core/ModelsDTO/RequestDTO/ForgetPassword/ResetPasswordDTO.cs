using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.RequestDTO.ForgetPassword
{
    public class ResetPasswordDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password",ErrorMessage ="Password And Confirm Password Not Matched")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
