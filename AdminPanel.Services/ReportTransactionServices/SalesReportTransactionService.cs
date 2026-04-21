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
    public class SalesReportTransactionService : ISalesReportTransactionService
    {
        #region services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<SalesReportTransactionService> _logger;
        protected readonly IServiceImport _serviceImport;

        public SalesReportTransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SalesReportTransactionService> logger, IServiceImport serviceImport)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _serviceImport = serviceImport;
        }
        #endregion

        #region Get All Reports Async
        public async Task<ResultServiceApplication<IReadOnlyList<SalesReportTransactionToReturnDTO>>> GetAllAsync()
        {
            try
            {
                var spec = new SalesReportSpec();
                var result = await _unitOfWork.CreateRepository<ReportTransaction>().GetAllAsyncSpec(spec);
                if (!result.Any()) return ResultServiceApplication<IReadOnlyList<SalesReportTransactionToReturnDTO>>.Fail("There are no sales reports to show");
                var data = _mapper.Map<IReadOnlyList<SalesReportTransactionToReturnDTO>>(result);
                return ResultServiceApplication<IReadOnlyList<SalesReportTransactionToReturnDTO>>.Success(data, "All sales reports retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<IReadOnlyList<SalesReportTransactionToReturnDTO>>.Fail("There is problem In Database");
            }
        }
        #endregion

        #region Get Details Report Async
        public async Task<ResultServiceApplication<SalesReportTransactionToReturnDTO>> GetDetailsSalesReportAsync(int id)
        {
            try
            {
                var spec = new SalesReportSpec(id);
                var result = await _unitOfWork.CreateRepository<ReportTransaction>().GetAsyncSpec(spec);
                if (result == null) return ResultServiceApplication<SalesReportTransactionToReturnDTO>.Fail("There are no sales reports to show");

                var data = _mapper.Map<SalesReportTransactionToReturnDTO>(result);
                return ResultServiceApplication<SalesReportTransactionToReturnDTO>.Success(data, "This is the sales report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<SalesReportTransactionToReturnDTO>.Fail("There is problem In Database");
            }
        }
        #endregion

        #region Add Report Async
        public async Task<ResultServiceApplication<SalesReportTransactionToReturnDTO>> AddSalesReportAsync(string userId, CreateSalesReportDTO dto)
        {
            try
            {
                var salesReportToSave = _mapper.Map<ReportTransaction>(dto);
                salesReportToSave.UserId = userId;
                salesReportToSave.CreatedBy = userId;
                salesReportToSave.CreatedAt = DateTime.UtcNow;

                foreach (var item in salesReportToSave.Items)
                {
                    if (item.ProductId.HasValue)
                    {
                        var product = await _unitOfWork.CreateRepository<Product>().GetAsync(item.ProductId.Value);
                        if (product != null)
                        {
                            var dtoItem = dto.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
                            if (dtoItem is null)
                            {
                                continue;
                            }

                            item.ProductId = product.Id;
                            item.ProductName = product.ProductName;
                            item.Price = dtoItem.Price;
                            item.GetTotalPrice();
                            product.Price = dtoItem.Price; // update product price
                            item.Product = product;
                            _unitOfWork.CreateRepository<Product>().Update(product);
                        }
                    }
                }

                salesReportToSave.GetTotalReportTransactionPrice();
                await _unitOfWork.CreateRepository<ReportTransaction>().AddAsync(salesReportToSave);
                await _unitOfWork.CompleteAsync();

                var salesReportToReturn = _mapper.Map<SalesReportTransactionToReturnDTO>(salesReportToSave);
                if (salesReportToReturn is null) return ResultServiceApplication<SalesReportTransactionToReturnDTO>.Fail("Sales report was not created");
                return ResultServiceApplication<SalesReportTransactionToReturnDTO>.Success(salesReportToReturn, "Sales report added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<SalesReportTransactionToReturnDTO>.Fail("There is problem In database");
            }
        }
        #endregion

        #region Delete Report Async
        public async Task<ResultServiceApplication<bool>> DeleteSalesReportAsync(int id)
        {
            var reportRepo = _unitOfWork.CreateRepository<ReportTransaction>();
            var report = await reportRepo.GetAsync(id);
            if(report is null) return ResultServiceApplication<bool>.Fail("Sales report not found");
            report.IsDeleted = true;
            reportRepo.Update(report);
            await _unitOfWork.CompleteAsync();
            return ResultServiceApplication<bool>.Success(true, "Sales report deleted successfully");
        }

        #endregion

        #region GetReportForExportAsync
        public async Task<IReadOnlyList<SalesReportTransactionExportToReturnDTO>> GetSalesReportForExportAsync()
        {
            var spec = new SalesReportSpec();
            var data = await _unitOfWork.CreateRepository<ReportTransaction>().GetAllAsyncSpec(spec);
            var mappingData = _mapper.Map<IReadOnlyList<SalesReportTransactionExportToReturnDTO>>(data);
            return mappingData;
        }

        public async Task<IReadOnlyList<SalesReportTransactionItemExportToReturnDTO>> GetSalesReportItemsForExportAsync()
        {
            var spec = new SalesReportSpec();
            var reports = await _unitOfWork.CreateRepository<ReportTransaction>().GetAllAsyncSpec(spec);

            var result = reports
                .SelectMany(report => report.Items.Select(item => new SalesReportTransactionItemExportToReturnDTO
                {
                    SalesReportId = report.Id,
                    ItemId = item.Id,
                    UserId = report.UserId,
                    UserName = report.ApplicationUser != null ? report.ApplicationUser.UserName! : report.CreatedBy,
                    CompanyName = report.CompanyName,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                }))
                .ToList();

            return result;
        }
        #endregion

        #region GetReportForImportAsync
        public async Task<ImportToReturnDTO<SalesReportImportRow>> GetSalesReportForImportAsync(ImportDTO<SalesReportTransactionToImport> req)
        {
            var importedRows = await _serviceImport.ExcelImportAsync(new ImportDTO<SalesReportTransactionToImport>
            {
                File = req.File,
                Config = BuildImportConfig<SalesReportTransactionToImport>("SalesReport")
            });

            var importedItems = await _serviceImport.ExcelImportAsync(new ImportDTO<SalesReportTransactionItemExportToReturnDTO>
            {
                File = req.File,
                Config = BuildImportConfig<SalesReportTransactionItemExportToReturnDTO>("SalesReportItems")
            });

            var reportRepo = _unitOfWork.CreateRepository<ReportTransaction>();
            var importedReportIds = importedRows.Data
                .Where(row => row.Id > 0)
                .Select(row => row.Id)
                .Distinct()
                .ToList();

            var existingReportIds = new HashSet<int>();
            if (importedReportIds.Any())
            {
                var existingReports = await reportRepo.GetAllAsyncSpec(new SalesReportSpec(importedReportIds));
                existingReportIds = existingReports
                    .Select(report => report.Id)
                    .ToHashSet();
            }

            var newRows = importedRows.Data
                .Where(row => row.Id <= 0 || !existingReportIds.Contains(row.Id))
                .ToList();

            var itemLookup = importedItems.Data
                .GroupBy(item => item.SalesReportId)
                .ToDictionary(group => group.Key, group => group.ToList());

            var salesReportsToSave = newRows.Select(row =>
            {
                var report = new ReportTransaction
                {
                    UserId = row.UserId,
                    CompanyName = row.CompanyName,
                    IsDeleted = row.IsDeleted,
                    CreatedBy = string.IsNullOrWhiteSpace(row.CreatedBy) ? row.UserName : row.CreatedBy,
                    ModifiedBy = row.ModifiedBy,
                    CreatedAt = row.CreatedAt == default ? DateTime.UtcNow : row.CreatedAt,
                    LastModifiedAt = row.LastModifiedAt == default ? DateTime.UtcNow : row.LastModifiedAt
                };

                if (itemLookup.TryGetValue(row.Id, out var reportItems))
                {
                    report.Items = reportItems.Select(item => new ReportTransactionItem
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    }).ToList();

                    report.GetTotalReportTransactionPrice();
                }
                else
                {
                    report.TotalReportTransaction = row.TotalReportTransactionPrice;
                }

                return report;
            }).ToList();

            if (salesReportsToSave.Any())
            {
                await reportRepo.AddRangeAsync(salesReportsToSave);
                await _unitOfWork.CompleteAsync();
            }

            var resultRows = newRows.Select(row => new SalesReportImportRow
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

            var errors = (importedRows.Errors ?? new List<string>())
                .Concat(importedItems.Errors ?? new List<string>())
                .Distinct()
                .ToList();
            var skippedReportsCount = importedRows.Data.Count - newRows.Count;
            if (skippedReportsCount > 0)
            {
                errors.Add($"{skippedReportsCount} existing sales report(s) were skipped during import.");
            }

            return new ImportToReturnDTO<SalesReportImportRow>
            {
                Data = resultRows,
                TotalRows = resultRows.Count,
                AddedCount = resultRows.Count,
                SkippedDuplicates = skippedReportsCount,
                Errors = errors
            };
        }

        private static ImportExcelConfiguration<T> BuildImportConfig<T>(string sheetName)
        {
            return new ImportExcelConfiguration<T>
            {
                SheetName = sheetName,
                StartRow = 2,
                HasHeader = true
            };
        }
        #endregion
    }
}
