using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Brands;
using AdminPanel.Core.ModelsDto.ResponseDTO.Brands;

namespace AdminPanel.Core.Service_Contract.brandsServices
{
    public interface IBrandService
    {
        Task<ResultServiceApplication<IReadOnlyList<BrandToReturnDTO>>> GetBrandsAsync();

        Task<ResultServiceApplication<BrandToReturnDTO>> GetBrandDetailsAsync(int id);
        Task<ResultServiceApplication<BrandToReturnDTO>> AddBrandAsync(CreatedBrandDTO brand);
        Task<ResultServiceApplication<BrandToReturnDTO>> UpdateBrandAsync(UpdatedBrandDTO brand);

        Task<ResultServiceApplication<bool>> DeleteBrandAsync(int  id);
    }
}
