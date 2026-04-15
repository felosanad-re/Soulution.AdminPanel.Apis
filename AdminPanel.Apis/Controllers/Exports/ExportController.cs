using AdminPanel.Core.ModelsDto.RequestDTO.Exports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Purchases;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AdminPanel.Core.Service_Contract.ExportServices;
using AdminPanel.Core.Service_Contract.ProductServices;
using AdminPanel.Core.Service_Contract.PurchaseServices;
using AdminPanel.Core.Service_Contract.ReportServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Exports
{
    public class ExportController : BaseController
    {
        protected readonly IExportService _exportService;
        protected readonly IProductService _productService;
        protected readonly IPurchaseService _purchaseService;
        protected readonly ISalesReportTransactionService _salesReportTransactionService;
        public ExportController(IExportService exportService, IProductService productService, IPurchaseService purchaseService, ISalesReportTransactionService salesReportTransactionService)
        {
            _exportService = exportService;
            _productService = productService;
            _purchaseService = purchaseService;
            _salesReportTransactionService = salesReportTransactionService;
        }

        #region Export Products
        [HttpGet("Products")] // Get: /api/Export/Products
        public async Task<IActionResult> ExportProducts()
        {
            var request = new ExportRequest<ProductExportToReturnDTO>
            {
                WorksheetName = "Products",
                DataFetcher = async () =>
                {
                    var data = await _productService.GetProductForExportAsync();
                    return data;
                }
            };
            var file = await _exportService.ExportAsync(request);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            request.WorksheetName);
        }
        #endregion

        #region Export Purchase
        [HttpGet("Purchase")] // Get: /api/Export/Purchase
        public async Task<IActionResult> ExportPurchase()
        {
            var request = new ExportRequest<PurchaseInvoiceExportToReturnDTO>
            {
                WorksheetName = "Purchase",
                DataFetcher = async () =>
                {
                    var data = await _purchaseService.GetPurchaseExport();
                    return data;
                }
            };

            var file = await _exportService.ExportAsync(request);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", request.WorksheetName);
        }
        #endregion

        #region Export Buyer
        [HttpGet("SalesReport")] // Get: /api/Export/SalesReport
        public async Task<IActionResult> ExportSalesReport()
        {
            var request = new ExportRequest<SalesReportTransactionExportToReturnDTO>
            {
                WorksheetName = "SalesReport",
                DataFetcher = async () =>
                {
                    return await _salesReportTransactionService.GetSalesReportForExportAsync();
                }
            };

            var file = await _exportService.ExportAsync(request);
            return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", request.WorksheetName);
        }
        #endregion
    }
}
