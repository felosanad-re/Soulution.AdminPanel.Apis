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
        [HttpGet("Products")] // Get: /api/Product/Products
        public async Task<ActionResult<IReadOnlyList<ProductToReturnDTO>>> GetAll([FromQuery]ProductParams @params)
        {
            var products = await _productService.GetAllAsync(@params);
            return Ok(products);
        }
        #endregion

        #region Get Product Details
        [HttpGet("productDetails/{id}")] // Get: /api/product/productDrails?id=
        public async Task<ActionResult<ProductToReturnDTO>> GetProductDetails(int id)
        {
            var product = await _productService.GetProductDetailsAsync(id)!;
            return Ok(product);
        }
        #endregion

        #region Add Product
        [HttpPost("AddProduct")] // Post: /api/product/AddProduct
        public async Task<ActionResult<ProductToReturnDTO>> AddProduct([FromForm]CreateProductDTO dTO)
        {
            var addProduct = await _productService.AddProductAsync(dTO);
            return Ok(addProduct);
        }
        #endregion

        #region Edit Product
        [HttpPut("editProduct")] // Post: /api/product/editProduct
        public async Task<ActionResult<ProductToReturnDTO>> EditProduct([FromForm]UpdateProductDTO dTO)
        {
            var editProduct = await _productService.UpdateProductAsync(dTO);
            return Ok(editProduct);
        }
        #endregion

        #region Delete Product
        [HttpDelete("deleteProduct/{id}")] //Delete: /api/product/deleteProduct
        public async Task<ActionResult> DeleteProduct([FromRoute]int id)
        {
            var product = await _productService.DeleteProductAsync(id);
            return Ok(product.Message);
        }
        #endregion

        #region Multiple Deleted
        [HttpDelete("bulk")] // Delete: /api/product/bulk?ids
        public async Task<ActionResult> MultipleDeleted([FromQuery]IEnumerable<int> ids)
        {
            var products = await _productService.DeleteBulkAsync(ids);
            if (!products.Succeed) return BadRequest(products.Errors);
            return Ok(products.Message);
        }
        #endregion
    }
}
