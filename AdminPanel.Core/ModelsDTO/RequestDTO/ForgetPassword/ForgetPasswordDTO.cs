using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.RequestDTO.ForgetPassword
{
    public class ForgetPasswordDTO
    {
        public string EmailOrUserName { get; set; } = string.Empty;
    }
}
