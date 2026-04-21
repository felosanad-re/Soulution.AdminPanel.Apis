using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.ModelsDto;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AdminPanel.Core.Service_Contract.AttachmentServices;
using AdminPanel.Core.Service_Contract.ImportServices;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.Specifications;
using AdminPanel.Core.Specifications.ProductSpecifications;
using AdminPanel.Core.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AdminPanel.Services.ProductServices
{
    public class ProductService : IProductService
    {
        #region Service
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAttachmentService _attachmentService;
        private readonly IServiceImport _serviceImport;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;
        private readonly IConfiguration _configuration;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProductService> logger, IAttachmentService attachmentService, IConfiguration configuration, IServiceImport serviceImport)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _attachmentService = attachmentService;
            _configuration = configuration;
            _serviceImport = serviceImport;
        }
        #endregion

        #region Get All Products
        public async Task<ResultServiceApplication<PaginationModel<ProductToReturnDTO>>> GetAllAsync(ProductParams @params)
        {
            var spec = new ProductSpec(@params);
            var products = await _unitOfWork.CreateRepository<Product>().GetAllAsyncSpec(spec);
            if (!products.Any()) return ResultServiceApplication<PaginationModel<ProductToReturnDTO>>.Fail("No Product To Show");
            var data = _mapper.Map<IReadOnlyList<ProductToReturnDTO>>(products);
            var count = await GetProductCount(@params);
            var pagination = new PaginationModel<ProductToReturnDTO>(
                @params.PageIndex,
                @params.PageSize,
                count,
                data
                );
            return ResultServiceApplication<PaginationModel<ProductToReturnDTO>>
                .Success(pagination, "There Is Products To Show");
        }
        #endregion

        #region Get Product Details
        public async Task<ResultServiceApplication<ProductToReturnDTO>> GetProductDetailsAsync(int id)
        {
            var spec = new ProductSpec(id);
            var product = await _unitOfWork.CreateRepository<Product>().GetAsyncSpec(spec);
            if (product == null) return ResultServiceApplication<ProductToReturnDTO>
                    .Fail("Product Not Found");
            var data = _mapper.Map<ProductToReturnDTO>(product);
            return ResultServiceApplication<ProductToReturnDTO>.Success(data, "Product Details Show Succeeded");
        }
        #endregion

        #region Add Product
        public async Task<ResultServiceApplication<ProductToReturnDTO>> AddProductAsync(CreateProductDTO productDTO)
        {
            try
            {
                var newProduct = new Product()
                {
                    ProductName = productDTO.ProductName,
                    BrandId = productDTO.BrandId,
                    CategoryId = productDTO.CategoryId,
                    Price = productDTO.Price,
                    Stock = productDTO.Stock,
                    Description = productDTO.Description,
                };

                // Upload Main Image
                var allowExtentions = _configuration.GetSection("FileSitteng:Allowed_Extentions").Get<string[]>();
                var maxSize = _configuration.GetValue<int>("FileSitteng:MaxSize");
                if(productDTO.MainImage != null)
                {
                    var folderName = _configuration["FileSitteng:ProductMainImages"];
                    newProduct.MainImage = await _attachmentService.UploadAsync(productDTO.MainImage, folderName, allowExtentions, maxSize);
                }

                // Upload Sub Images
                if(productDTO.SubImages != null && productDTO.SubImages.Any())
                {
                    var folderName = _configuration["FileSitteng:ProductSubImages"];
                    var imagesUrl = await _attachmentService.UploadsAsync(productDTO.SubImages,
                        folderName,
                        allowExtentions,
                        maxSize);
                    newProduct.SubImages = imagesUrl.Select(url => new ProductImages { ImagesUrl = url }).ToList();
                }

                await _unitOfWork.CreateRepository<Product>().AddAsync(newProduct);
                await _unitOfWork.CompleteAsync();

                var data = _mapper.Map<ProductToReturnDTO>(newProduct);
                return ResultServiceApplication<ProductToReturnDTO>
                .Success(data, "Product Created Succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<ProductToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Update Product
        public async Task<ResultServiceApplication<ProductToReturnDTO>> UpdateProductAsync(UpdateProductDTO updatedProduct)
        {
            try
            {
                var allowExtentions = _configuration.GetSection("FileSitteng:Allowed_Extentions").Get<string[]>();
                var maxSize = _configuration.GetValue<int>("FileSitteng:MaxSize");
                var productRepo = _unitOfWork.CreateRepository<Product>();
                var product = await productRepo.GetAsync(updatedProduct.Id);
                if (product == null) return ResultServiceApplication<ProductToReturnDTO>
                    .Fail("There Is No Product Found");

                product.ProductName = updatedProduct.ProductName;
                product.BrandId = updatedProduct.BrandId;
                product.CategoryId = updatedProduct.CategoryId;
                product.Price = updatedProduct.Price;
                product.Stock = updatedProduct.Stock;
                product.Description = updatedProduct.Description;

                // Edit Main Image
                if(updatedProduct.MainImage != null)
                {
                    // Delete Old Image
                    await _attachmentService.DeleteImageAsync(product.MainImage, _configuration["FileSitteng:ProductMainImages"]!);
                    var folderName = _configuration["FileSitteng:ProductMainImages"];
                    product.MainImage = await _attachmentService.UploadAsync(updatedProduct.MainImage, folderName, allowExtentions, maxSize);
                }
                // Edit Sub Images
                if(updatedProduct.SubImages != null && updatedProduct.SubImages.Any())
                {
                    var folderName = _configuration["FileSitteng:ProductSubImages"];
                    // Delete Old Images
                    foreach (var oldImage in product.SubImages)
                    {
                        if (!string.IsNullOrEmpty(oldImage.ImagesUrl))
                            await _attachmentService.DeleteImageAsync(oldImage.ImagesUrl, folderName!);
                    }

                    var imagesUrl = await _attachmentService.UploadsAsync(updatedProduct.SubImages, folderName, allowExtentions, maxSize);
                    product.SubImages = imagesUrl.Select(url => new ProductImages { ImagesUrl = url }).ToList();
                }
                productRepo.Update(product);
                await _unitOfWork.CompleteAsync();
                var data = _mapper.Map<ProductToReturnDTO>(product);
                return ResultServiceApplication<ProductToReturnDTO>
                    .Success(data, "Product Edit Succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<ProductToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Delete Multi Products
        public async Task<ResultServiceApplication<bool>> DeleteBulkAsync(List<int> productsId)
        {
            try
            {
                if (!productsId.Any())
                    return ResultServiceApplication<bool>.Fail("No Products Selected");

                var productRepo = _unitOfWork.CreateRepository<Product>();
                var spec = new ProductSpec(productsId);
                var items = await productRepo.GetAllAsyncSpec(spec);
                if (!items.Any()) return ResultServiceApplication<bool>
                        .Fail("No Products To Show");
                foreach (var item in items)
                {
                    productRepo.Delete(item);
                }
                await _unitOfWork.CompleteAsync();
                return ResultServiceApplication<bool>.Success(true, "Products Deleted Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<bool>.Fail("Error In Database");
            }
        }
        #endregion

        #region Delete Product
        public async Task<ResultServiceApplication<ProductToReturnDTO>> DeleteProductAsync(int id)
        {
            try
            {
                var productRepo = _unitOfWork.CreateRepository<Product>();
                var product = await productRepo.GetAsync(id);
                if (product == null) return ResultServiceApplication<ProductToReturnDTO>
                    .Fail("Product Not Found");
                product.IsDeleted = true;
                productRepo.Update(product);
                await _unitOfWork.CompleteAsync();
                var data = _mapper.Map<ProductToReturnDTO>(product);
                return ResultServiceApplication<ProductToReturnDTO>
                    .Success(data, "Product Deleted Succeeded");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, ex.Message);
                return ResultServiceApplication<ProductToReturnDTO>.Fail("There Is a Problem With Deleted Product");
            }
        }
        #endregion

        #region Get Product Count
        public async Task<int> GetProductCount(ProductParams @params)
        {
            var spec = new ProductWithFilterSpec(@params);
            var productCount = await _unitOfWork.CreateRepository<Product>().GetCountAsyncSpec(spec);
            return productCount;
        }
        #endregion

        #region GetProductForExportAsync
        public async Task<IReadOnlyList<ProductExportToReturnDTO>> GetProductForExportAsync()
        {
            var spec = new ProductSpec();
            var data = await _unitOfWork.CreateRepository<Product>().GetAllAsyncSpec(spec);
            var dataMapping = _mapper.Map<IReadOnlyList<ProductExportToReturnDTO>>(data);
            return dataMapping;
        }

        public async Task<IReadOnlyList<ProductImageExportToReturnDTO>> GetProductImagesForExportAsync()
        {
            var spec = new ProductSpec();
            var products = await _unitOfWork.CreateRepository<Product>().GetAllAsyncSpec(spec);

            var result = products
                .SelectMany(product =>
                {
                    var images = new List<ProductImageExportToReturnDTO>();

                    if (!string.IsNullOrWhiteSpace(product.MainImage))
                    {
                        images.Add(new ProductImageExportToReturnDTO
                        {
                            ProductId = product.Id,
                            ProductName = product.ProductName,
                            ImageType = "Main",
                            ImageUrl = product.MainImage
                        });
                    }

                    if (product.SubImages != null)
                    {
                        images.AddRange(product.SubImages
                            .Where(image => !string.IsNullOrWhiteSpace(image.ImagesUrl))
                            .Select(image => new ProductImageExportToReturnDTO
                            {
                                ProductId = product.Id,
                                ProductName = product.ProductName,
                                ImageType = "Thumbnail",
                                ImageUrl = image.ImagesUrl
                            }));
                    }

                    return images;
                })
                .ToList();

            return result;
        }
        #endregion

        #region GetProductForImport
        public async Task<ImportToReturnDTO<ProductToImport>> GetProductForImport(ImportDTO<ProductToImport> req)
        {
            var productImport = await _serviceImport.ExcelImportAsync(new ImportDTO<ProductToImport>
            {
                File = req.File,
                Config = BuildImportConfig<ProductToImport>("Products")
            });

            var productImagesImport = await _serviceImport.ExcelImportAsync(new ImportDTO<ProductImageExportToReturnDTO>
            {
                File = req.File,
                Config = BuildImportConfig<ProductImageExportToReturnDTO>("ProductImages")
            });

            var excelRows = productImport.Data;
            var imageLookup = BuildProductImageLookup(productImagesImport.Data);

            var productToSave = new List<Product>(); // To Save In DB
            var importProduct = new List<ProductToImport>(); // To Return
            var productRepo = _unitOfWork.CreateRepository<Product>();
            // Unique Column
            var exsistProductsId = excelRows
                .Where(x => x.Id > 0)
                .Select(r => r.Id)
                .Distinct()
                .ToList();
            var exsistProducts = new List<Product>();
            if (exsistProductsId.Any())
            {
                var spec = new ProductSpec(exsistProductsId);
                var readOnlyList = await productRepo.GetAllAsyncSpec(spec);
                exsistProducts = readOnlyList.ToList(); // Convert From ReadOnlyList To List
            }
            var exsistingDigit = exsistProducts.ToDictionary(p => p.Id); // Set ProductId In Dictionary

            foreach (var row in excelRows)
            {
                var productImages = GetProductImages(imageLookup, row.Id, row.ProductName);

                // Update for data
                if(row.Id > 0 && exsistingDigit.TryGetValue(row.Id, out var existing))
                {
                    _mapper.Map(row, existing); // Update Mapping
                    ApplyProductImages(existing, productImages);
                    importProduct.Add(row);
                }
                // Add Products
                else
                {
                    var newProducts = _mapper.Map<Product>(row);
                    newProducts.Id = 0; // Let SQL Server generate the identity value for new rows.
                    ApplyProductImages(newProducts, productImages);
                    productToSave.Add(newProducts);
                    importProduct.Add(row);
                }
            }

            // Save Changes
            if (productToSave.Any())
            {
                await productRepo.AddRangeAsync(productToSave);
            }

            await _unitOfWork.CompleteAsync();
            return new ImportToReturnDTO<ProductToImport>
            {
                Data = importProduct,
                TotalRows = excelRows.Count,
                Errors = productImport.Errors
                    .Concat(productImagesImport.Errors)
                    .Distinct()
                    .ToList()
            };
        }

        #region helper methods
        private static ImportExcelConfiguration<T> BuildImportConfig<T>(string sheetName)
        {
            return new ImportExcelConfiguration<T>
            {
                SheetName = sheetName,
                StartRow = 2,
                HasHeader = true
            };
        }

        private static Dictionary<string, List<ProductImageExportToReturnDTO>> BuildProductImageLookup(IEnumerable<ProductImageExportToReturnDTO> imageRows)
        {
            return imageRows
                .GroupBy(image => BuildLookupKey(image.ProductId, image.ProductName))
                .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.OrdinalIgnoreCase);
        }

        private static List<ProductImageExportToReturnDTO> GetProductImages(
            IReadOnlyDictionary<string, List<ProductImageExportToReturnDTO>> imageLookup,
            int productId,
            string productName)
        {
            if (imageLookup.TryGetValue(BuildLookupKey(productId, productName), out var images))
            {
                return images;
            }

            return new List<ProductImageExportToReturnDTO>();
        }

        private static void ApplyProductImages(Product product, IEnumerable<ProductImageExportToReturnDTO> images)
        {
            var imageList = images.ToList();
            var mainImage = imageList
                .FirstOrDefault(image => image.ImageType.Equals("Main", StringComparison.OrdinalIgnoreCase))
                ?.ImageUrl;

            product.MainImage = string.IsNullOrWhiteSpace(mainImage) ? string.Empty : mainImage;

            product.SubImages = imageList
                .Where(image =>
                    image.ImageType.Equals("Thumbnail", StringComparison.OrdinalIgnoreCase) &&
                    !string.IsNullOrWhiteSpace(image.ImageUrl))
                .Select(image => new ProductImages
                {
                    Product = product,
                    ImagesUrl = image.ImageUrl,
                })
                .ToList();
        }

        private static string BuildLookupKey(int id, string? name)
        {
            if (id > 0)
            {
                return $"id:{id}";
            }

            return $"name:{name?.Trim().ToLowerInvariant()}";
        }
        #endregion
        #endregion
    }
}
