using AdminPanel.Apis.Errors_Handler;
using AdminPanel.Core.ModelsDto;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Products
{
    public class ProductController : BaseController
    {
        #region Services
        protected readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        #region Get All Product
        [ProducesResponseType(typeof(ResultServiceApplication<PaginationModel<ProductToReturnDTO>>), 200)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [HttpGet("Products")] // Get: /api/Product/Products
        public async Task<ActionResult<ResultServiceApplication<PaginationModel<ProductToReturnDTO>>>> GetAll([FromQuery]ProductParams @params)
        {
            var products = await _productService.GetAllAsync(@params);
            if (!products.Succeed)
                return BadRequest(products.Errors);
            return Ok(products);
        }
        #endregion

        #region Get Product Details
        [HttpGet("productDetails/{id}")] // Get: /api/product/productDetails/id
        public async Task<ActionResult<ResultServiceApplication<ProductToReturnDTO>>> GetProductDetails(int id)
        {
            var product = await _productService.GetProductDetailsAsync(id);
            if(!product.Succeed) return BadRequest(product.Errors);
            return Ok(product);
        }
        #endregion

        #region Add Product
        [HttpPost("AddProduct")] // Post: /api/product/AddProduct
        public async Task<ActionResult<ResultServiceApplication<ProductToReturnDTO>>> AddProduct([FromForm]CreateProductDTO dTO)
        {
            var addProduct = await _productService.AddProductAsync(dTO);
            if(!addProduct.Succeed) return BadRequest(addProduct.Errors);
            return Ok(addProduct);
        }
        #endregion

        #region Edit Product
        [HttpPut("editProduct")] // Post: /api/product/editProduct
        public async Task<ActionResult<ResultServiceApplication<ProductToReturnDTO>>> EditProduct([FromForm]UpdateProductDTO dTO)
        {
            var editProduct = await _productService.UpdateProductAsync(dTO);
            if (!editProduct.Succeed) return BadRequest(editProduct.Errors);
            return Ok(editProduct);
        }
        #endregion

        #region Delete Product
        [HttpDelete("deleteProduct/{id}")] //Delete: /api/product/deleteProduct
        public async Task<ActionResult<ResultServiceApplication<ProductToReturnDTO>>> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProductAsync(id);
            if (!product.Succeed) return BadRequest(product.Message);
            return Ok(product.Message);
        }
        #endregion

        #region Multiple Deleted
        [HttpPost("bulk")] // Delete: /api/product/bulk
        public async Task<ActionResult<ResultServiceApplication<ProductToReturnDTO>>> MultipleDeleted([FromBody]List<int> ids)
        {
            if (ids.Count > 25) return BadRequest(ResultServiceApplication<ProductToReturnDTO>.Fail("You Can Only Choose 25 Product"));
            var products = await _productService.DeleteBulkAsync(ids);
            if (!products.Succeed) return BadRequest(products.Errors);
            return Ok(products);
        }
        #endregion
    }
}
