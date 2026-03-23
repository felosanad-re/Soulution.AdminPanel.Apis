using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Brands;
using AdminPanel.Core.ModelsDto.ResponseDTO.Brands;
using AdminPanel.Core.Service_Contract.brandsServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Brands
{
    public class BrandController : BaseController
    {
        protected readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        // Get All
        [HttpGet("Brands")] //Get: /api/brand/brands
        public async Task<ActionResult<ResultServiceApplication<IReadOnlyList<BrandToReturnDTO>>>> Get()
        {
            var result = await _brandService.GetBrandsAsync();
            if(!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpGet("BrandDetails/{id}")] //Get: /api/brand/BrandDetails/{id}
        public async Task<ActionResult<ResultServiceApplication<BrandToReturnDTO>>> GetBrandDetails(int id)
        {
            var result = await _brandService.GetBrandDetailsAsync(id);
            if(!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("AddBrand")] //Get: /api/brand/AddBrand
        public async Task<ActionResult<ResultServiceApplication<BrandToReturnDTO>>> AddBrand([FromForm]CreatedBrandDTO dTO)
        {
            var result = await _brandService.AddBrandAsync(dTO);
            if(!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPut("EditBrand")] //Get: /api/brand/EditBrand
        public async Task<ActionResult<ResultServiceApplication<BrandToReturnDTO>>> UpdateBrand([FromForm]UpdatedBrandDTO dTO)
        {
            var result = await _brandService.UpdateBrandAsync(dTO);
            if(!result.Succeed) return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpDelete("DeletedBrand/{id}")] //Get: /api/brand/DeletedBrand
        public async Task<ActionResult<ResultServiceApplication<bool>>> DeleteBrand(int id)
        {
            var result = await _brandService.DeleteBrandAsync(id);
            if(!result.Succeed) return BadRequest(result.Errors);

            return Ok(result.Message);
        }
    }
}
