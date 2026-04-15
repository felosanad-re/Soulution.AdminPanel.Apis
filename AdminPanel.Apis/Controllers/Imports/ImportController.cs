using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.Service_Contract.PurchaseServices;
using AdminPanel.Core.Service_Contract.ReportServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Imports
{
    public class ImportController : BaseController
    {
        protected readonly ISalesReportTransactionService _salesReportService;
        protected readonly IProductService _productService;
        protected readonly IPurchaseService _purchaseService;

        public ImportController(ISalesReportTransactionService salesReportService, IProductService productService, IPurchaseService purchaseService)
        {
            _salesReportService = salesReportService;
            _productService = productService;
            _purchaseService = purchaseService;
        }

        #region Import Products
        [HttpPost("Products")] // Post: /api/Import/Products
        public async Task<ActionResult<ImportToReturnDTO<ProductToImport>>> ImportProducts([FromForm] ImportDTO<ProductToImport> request)
        {
            var productToImport = await _productService.GetProductForImport(request);
            return Ok(productToImport);
        }
        #endregion

        #region Import Purchase
        [HttpPost("Purchase")] // Post: /api/Import/Purchase
        public async Task<ActionResult<ImportToReturnDTO<PurchaseInvoiceToImport>>> ImportPurchase([FromForm] ImportDTO<PurchaseInvoiceToImport> req)
        {
            if (req?.File == null || req.File.Length == 0)
            {
                return BadRequest(new ImportToReturnDTO<PurchaseInvoiceToImport>
                {
                    Errors = new List<string> { "Excel File is required" }
                });
            }

            // Keep the controller thin and let the purchase service own the import flow.
            var data = await _purchaseService.GetPurchaseForImportAsync(req);
            return Ok(data);
        }
        #endregion

        #region Import Buyer
        [HttpPost("SalesReport")] // Post: /api/Import/SalesReport
        public async Task<ActionResult<ImportToReturnDTO<SalesReportImportRow>>> ImportSalesReport([FromForm] ImportDTO<SalesReportTransactionToImport> req)
        {
            if (req.File == null || req.File.Length == 0)
            {
                return BadRequest(new ImportToReturnDTO<SalesReportImportRow>
                {
                    Errors = new List<string> { "Excel File is required" }
                });
            }

            var data = await _salesReportService.GetSalesReportForImportAsync(req);
            return Ok(data);
        }
        #endregion
    }
}
