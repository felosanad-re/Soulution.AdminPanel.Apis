using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Brands
{
    public class BrandToReturnDTO
    {
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}
