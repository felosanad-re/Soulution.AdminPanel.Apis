using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.ModelsDto;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AdminPanel.Core.Specifications;

namespace AdminPanel.Core.Service_Contract.ProductServices
{
    public interface IProductService
    {
        // Get All Product
        Task<ResultServiceApplication<PaginationModel<ProductToReturnDTO>>> GetAllAsync(ProductParams @params);
        // Get Product By Id
        Task<ResultServiceApplication<ProductToReturnDTO>>? GetProductDetailsAsync(int id);

        // Add Product
        Task<ResultServiceApplication<ProductToReturnDTO>> AddProductAsync(CreateProductDTO productDTO);
        // Update Product
        Task<ResultServiceApplication<ProductToReturnDTO>> UpdateProductAsync(UpdateProductDTO updatedProduct);
        // Delete one product
        Task<ResultServiceApplication<ProductToReturnDTO>> DeleteProductAsync(int id);
        // Delete Bulk Products
        Task<ResultServiceApplication<bool>> DeleteBulkAsync(List<int> productsId);
        Task<int> GetProductCount(ProductParams @params);

        Task<IReadOnlyList<ProductExportToReturnDTO>> GetProductForExportAsync();
    }
}
