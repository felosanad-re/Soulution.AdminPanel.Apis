using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AdminPanel.Core.Service_Contract.ImportServices;
using AdminPanel.Core.Service_Contract.ReportServices;
using AdminPanel.Core.Specifications.ReportSpecifications;
using AdminPanel.Core.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AdminPanel.Services.ReportTransactionServices
{
    public class ReportTransactionService : IReportTransactionService
    {
        #region services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<ReportTransactionService> _logger;
        protected readonly IServiceImport _serviceImport;

        public ReportTransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReportTransactionService> logger, IServiceImport serviceImport)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _serviceImport = serviceImport;
        }
        #endregion

        #region Get All Reports Async
        public async Task<ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>> GetAllAsync()
        {
            try
            {
                var spec = new ReportSpec();
                var result = await _unitOfWork.CreateRepository<ReportTransaction>().GetAllAsyncSpec(spec);
                if (!result.Any()) return ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>.Fail("There is no reports to show");
                var data = _mapper.Map<IReadOnlyList<ReportTransactionToReturnDTO>>(result);
                return ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>.Success(data, "All reports retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>.Fail("There is problem In Database");
            }
        }
        #endregion

        #region Get Details Report Async
        public async Task<ResultServiceApplication<ReportTransactionToReturnDTO>> GetDetailsReportAsync(int id)
        {
            try
            {
                var spec = new ReportSpec(id);
                var result = await _unitOfWork.CreateRepository<ReportTransaction>().GetAsyncSpec(spec);
                if (result == null) return ResultServiceApplication<ReportTransactionToReturnDTO>.Fail("There is no reports to show");

                var data = _mapper.Map<ReportTransactionToReturnDTO>(result);
                return ResultServiceApplication<ReportTransactionToReturnDTO>.Success(data, "This Is Report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<ReportTransactionToReturnDTO>.Fail("There is problem In Database");
            }
        }
        #endregion

        #region Add Report Async
        public async Task<ResultServiceApplication<ReportTransactionToReturnDTO>> AddReportAsync(string userId, CreateReportDTO dTO)
        {
            try
            {
                var newReport = _mapper.Map<ReportTransaction>(dTO);
                newReport.UserId = userId;
                newReport.CreatedBy = userId;
                newReport.CreatedAt = DateTime.UtcNow;
                // Get items
                foreach (var item in newReport.Items)
                {
                    if(item.ProductId.HasValue)
                    {
                        var product = await _unitOfWork.CreateRepository<Product>().GetAsync(item.ProductId.Value);
                        if (product != null)
                        {
                            var dtoItem = dTO.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
                            item.ProductId = product.Id;
                            item.ProductName = product.ProductName;
                            item.Price = dtoItem.Price;
                            product.Price = dtoItem.Price; // update product price
                            item.Product = product;
                            _unitOfWork.CreateRepository<Product>().Update(product);
                        }
                    }
                }
                newReport.GetTotalReportTransactionPrice(); // Calc Total Purchase
                await _unitOfWork.CreateRepository<ReportTransaction>().AddAsync(newReport);
                await _unitOfWork.CompleteAsync();

                var result = _mapper.Map<ReportTransactionToReturnDTO>(newReport);
                if (result is null) return ResultServiceApplication<ReportTransactionToReturnDTO>.Fail("Report Not Created");
                return ResultServiceApplication<ReportTransactionToReturnDTO>.Success(result, "Report Added Succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<ReportTransactionToReturnDTO>.Fail("There is problem In database");
            }
        }
        #endregion

        #region Delete Report Async
        public async Task<ResultServiceApplication<bool>> DeleteReportAsync(int id)
        {
            var reportRepo = _unitOfWork.CreateRepository<ReportTransaction>();
            var report = await reportRepo.GetAsync(id);
            if(report is null) return ResultServiceApplication<bool>.Fail("Report Not Found");
            report.IsDeleted = true;
            reportRepo.Update(report);
            await _unitOfWork.CompleteAsync();
            return ResultServiceApplication<bool>.Success(true,"Report Deleted succeeded");
        }

        #endregion

        #region GetReportForExportAsync
        public async Task<IReadOnlyList<ReportTransactionExportToReturnDTO>> GetReportForExportAsync()
        {
            var spec = new ReportSpec();
            var data = await _unitOfWork.CreateRepository<ReportTransaction>().GetAllAsyncSpec(spec);
            var mappingData = _mapper.Map<IReadOnlyList<ReportTransactionExportToReturnDTO>>(data);
            return mappingData;
        }
        #endregion

        #region GetReportForImportAsync
        public async Task<ImportToReturnDTO<BuyerToReturnRow>> GetReportForImportAsync(ImportDTO<ReportTransactionToImport> req)
        {
            // Read report rows from Excel using the shared generic import logic.
            var importedRows = await _serviceImport.ExcelImportAsync(req);

            var reportRepo = _unitOfWork.CreateRepository<ReportTransaction>();
            var importedReportIds = importedRows.Data
                .Where(row => row.Id > 0)
                .Select(row => row.Id)
                .Distinct()
                .ToList();

            var existingReportIds = new HashSet<int>();
            if (importedReportIds.Any())
            {
                var existingReports = await reportRepo.GetAllAsyncSpec(new ReportSpec(importedReportIds));
                existingReportIds = existingReports
                    .Select(report => report.Id)
                    .ToHashSet();
            }

            var newRows = importedRows.Data
                .Where(row => row.Id <= 0 || !existingReportIds.Contains(row.Id))
                .ToList();

            // Shape the imported data exactly like the response expected by the Buyer import endpoint.
            var resultRows = newRows.Select(row => new BuyerToReturnRow
            {
                Id = row.Id,
                UserName = row.UserName,
                UserId = row.UserId,
                CompanyName = row.CompanyName,
                Items = row.Items,
                TotalReportTransactionPrice = row.TotalReportTransactionPrice,
                IsDeleted = row.IsDeleted,
                CreatedAt = row.CreatedAt,
                LastModifiedAt = row.LastModifiedAt,
                CreatedBy = row.CreatedBy,
                ModifiedBy = row.ModifiedBy
            }).ToList();

            var errors = importedRows.Errors ?? new List<string>();
            var skippedReportsCount = importedRows.Data.Count - newRows.Count;
            if (skippedReportsCount > 0)
            {
                errors.Add($"{skippedReportsCount} existing report(s) were skipped during import.");
            }

            return new ImportToReturnDTO<BuyerToReturnRow>
            {
                Data = resultRows,
                TotalRows = resultRows.Count,
                Errors = errors
            };
        }
        #endregion
    }
}
