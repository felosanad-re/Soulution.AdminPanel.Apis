using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AdminPanel.Core.Service_Contract.ImportServices;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.Service_Contract.PurchaseServices;
using AdminPanel.Core.Service_Contract.ReportServices;
using AdminPanel.Core.Specifications;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Imports
{
    public class ImportController : BaseController
    {
        protected readonly IServiceImport _importService;
        protected readonly IReportTransactionService _reportService;
        protected readonly IProductService _productService;
        protected readonly IPurchaseService _purchaseService;

        public ImportController(IServiceImport importService, IReportTransactionService reportService, IProductService productService, IPurchaseService purchaseService)
        {
            _importService = importService;
            _reportService = reportService;
            _productService = productService;
            _purchaseService = purchaseService;
        }

        #region Import Products
        [HttpPost("Products")] // Post: /api/Import/Products
        public async Task<ActionResult<ImportToReturnDTO<ProductToImport>>> ImportProducts([FromForm] ImportDTO<Product> request)
        {
            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new ImportToReturnDTO<ProductToImport>
                {
                    Errors = new List<string> { "Excel File is required" }
                });
            }
            var products = await _productService.GetAllAsync(new ProductParams());
            var data = await _importService.ExcelImportAsync<Product>(request);
            var result = data.Data.Select(x =>
            {
                var product = products.Data.Data.FirstOrDefault(p => p.Id == x.Id);
                return new ProductToImport
                {
                    Id = x.Id,
                    BrandId = x.BrandId.Value,
                    CategoryId = x.CategoryId.Value,
                    Description = x.Description,
                    MainImage = x.MainImage,
                    SubImages = x.SubImages != null ? string.Join(" And ", x.SubImages.Select(p => p.ImagesUrl)): "",
                    Price = x.Price,
                    Stock = x.Stock,
                    ProductName = x.ProductName,
                    Type = x.Type.ToString(),
                    BrandName =x.Brand.BrandName,
                    CategoryName = x.Category.CategoryName
                };
            });
            return Ok(result);
        }
        #endregion

        #region Import Purchase
        //[HttpPost("Purchase")] // Post: /api/Import/Purchase
        //public async Task<ActionResult<ImportToReturnDTO>> ImportPurchase(ImportDTO<PurchaseInvoice> req)
        //{
        //    if (req?.File == null || req.File.Length == 0)
        //    {
        //        return BadRequest(new ImportToReturnDTO
        //        {
        //            Errors = new List<string> { "Excel File is required" }
        //        });
        //    }
        //    var data = await _importService.ExcelImportAsync<PurchaseInvoice>(req);
        //    if (data is null) return BadRequest(data);
        //    return Ok(data);
        //}
        #endregion

        #region Import Buyer
        [HttpPost("Buyer")] // Post: /api/Import/Buyer
        public async Task<ActionResult<ImportToReturnDTO<BuyerToReturnRow>>> ImportBuyer([FromForm]ImportDTO<ReportTransaction> req)
        {
            if(req.File == null || req.File.Length == 0)
            {
                return BadRequest(new ImportToReturnDTO<BuyerToReturnRow>
                {
                    Errors = new List<string> { "Excel File is required" }
                });
            }
            var allBuyers = await _reportService.GetAllAsync();
            if (!allBuyers.Succeed) return BadRequest();
            var data = await _importService.ExcelImportAsync<ReportTransaction>(req);
            var result = data.Data.Select(x =>
            {
                var report = allBuyers.Data.FirstOrDefault(r => r.Id == x.Id);
                return new BuyerToReturnRow
                {
                    Id = x.Id,
                    TotalReportTransactionPrice = x.TotalReportTransaction,
                    UserName = x.UserId,
                    Items = x.Items != null ? (string.Join(" And ", report?.Items.Select(i => i.ProductName))) : ""
                };
            });
            if (data is null) return BadRequest();

            return Ok(result);
        }
        #endregion
    }
}
