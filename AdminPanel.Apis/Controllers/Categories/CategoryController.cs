using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Categories;
using AdminPanel.Core.ModelsDto.ResponseDTO.Categories;
using AdminPanel.Core.Service_Contract.CategoriesServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Categories
{

    public class CategoryController : BaseController
    {
        protected readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        // Get All
        [HttpGet("Categories")] //Get: /api/Category/Categories
        public async Task<ActionResult<ResultServiceApplication<IReadOnlyList<CategoryToReturnDTO>>>> Get()
        {
            var result = await _categoryService.GetAllAsync();
            if (!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("CategoryDetails/{id}")] //Get: /api/Category/CategoryDetails/{id}
        public async Task<ActionResult<ResultServiceApplication<CategoryToReturnDTO>>> GetCategoryDetails(int id)
        {
            var result = await _categoryService.GetCategoryDetailsAsync(id);
            if (!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("AddCategory")] //Get: /api/Category/AddCategory
        public async Task<ActionResult<ResultServiceApplication<CategoryToReturnDTO>>> AddCategory([FromForm]CreatedCategoryDTO dTO)
        {
            var result = await _categoryService.AddCategoryAsync(dTO);
            if (!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("EditCategory")] //Get: /api/Category/EditCategory
        public async Task<ActionResult<ResultServiceApplication<CategoryToReturnDTO>>> UpdateCategory([FromForm] UpdatedCategoryDTO dTO)
        {
            var result = await _categoryService.UpdatedCategoryAsync(dTO);
            if (!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpDelete("DeletedCategory/{id}")] //Get: /api/Category/DeletedBrand
        public async Task<ActionResult<ResultServiceApplication<bool>>> DeleteBrand(int id)
        {
            var result = await _categoryService.DeletedCategory(id);
            if (!result.Succeed) return BadRequest(result.Errors);

            return Ok(result.Message);
        }
    }
}
