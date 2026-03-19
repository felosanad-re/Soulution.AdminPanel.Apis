using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Categories;
using AdminPanel.Core.ModelsDto.ResponseDTO.Categories;

namespace AdminPanel.Core.Service_Contract.CategoriesServices
{
    public interface ICategoryService
    {
        Task<ResultServiceApplication<IReadOnlyList<CategoryToReturnDTO>>> GetAllAsync();

        Task<ResultServiceApplication<CategoryToReturnDTO>> GetCategoryDetailsAsync(int id);
        Task<ResultServiceApplication<CategoryToReturnDTO>> AddCategoryAsync(CreatedCategoryDTO dTO);
        Task<ResultServiceApplication<CategoryToReturnDTO>> UpdatedCategoryAsync(UpdatedCategoryDTO dTO);
        Task<ResultServiceApplication<bool>> DeletedCategory(int id);

    }
}
