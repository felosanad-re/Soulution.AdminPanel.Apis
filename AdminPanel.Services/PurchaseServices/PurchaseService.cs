using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Import;
using AdminPanel.Core.ModelsDto.RequestDTO.Purchases;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Purchases;
using AdminPanel.Core.Service_Contract.ImportServices;
using AdminPanel.Core.Service_Contract.PurchaseServices;
using AdminPanel.Core.Specifications.PurchaseSpecifications;
using AdminPanel.Core.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AdminPanel.Services.PurchaseServices
{
    public class PurchaseService : IPurchaseService
    {
        #region Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<PurchaseService> _logger;
        protected readonly IServiceImport _serviceImport;
        public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PurchaseService> logger, IServiceImport serviceImport)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _serviceImport = serviceImport;
        }
        #endregion

        #region Get All Async
        public async Task<ResultServiceApplication<IReadOnlyList<PurchaseInvoiceToReturnDTO>>> GetAllAsync()
        {
            try
            {
                var spec = new PurchaseSpec();
                var purchase = await _unitOfWork.CreateRepository<PurchaseInvoice>().GetAllAsyncSpec(spec);
                if (purchase == null || !purchase.Any()) return ResultServiceApplication<IReadOnlyList<PurchaseInvoiceToReturnDTO>>.Fail("There Is No Purchase Reports To Show");
                var data = _mapper.Map<IReadOnlyList<PurchaseInvoiceToReturnDTO>>(purchase);
                return ResultServiceApplication<IReadOnlyList<PurchaseInvoiceToReturnDTO>>.Success(data, "This All purchase reports");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving purchase invoice");
                return ResultServiceApplication<IReadOnlyList<PurchaseInvoiceToReturnDTO>>.Fail("An error occurred while processing your request");
            }
        }
        #endregion

        #region Get Async
        public async Task<ResultServiceApplication<PurchaseInvoiceToReturnDTO>> GetAsync(int id)
        {
            try
            {
                var spec = new PurchaseSpec(id);
                var purchase = await _unitOfWork.CreateRepository<PurchaseInvoice>().GetAsyncSpec(spec);
                if (purchase == null) return ResultServiceApplication<PurchaseInvoiceToReturnDTO>.Fail("There Is No Purchase Reports To Show");

                var data = _mapper.Map<PurchaseInvoiceToReturnDTO>(purchase);
                return ResultServiceApplication<PurchaseInvoiceToReturnDTO>.Success(data, "This Is Purchase Reports To Show");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving purchase invoice with ID {PurchaseInvoiceId}", id);
                return ResultServiceApplication<PurchaseInvoiceToReturnDTO>.Fail("There is a warning in database");
            }
        }
        #endregion

        #region Add PurchaseAsync
        public async Task<ResultServiceApplication<PurchaseInvoiceToReturnDTO>> AddPurchaseAsync(CreatePurchaseDTO dto, string userName)
        {
            try
            {
                var productRepo = _unitOfWork.CreateRepository<Product>();
                var purchaseToSave = _mapper.Map<PurchaseInvoice>(dto);
                purchaseToSave.CreatedBy = userName; // Admin Account
                purchaseToSave.UserName = userName; // Admin Name
                purchaseToSave.CreatedAt = DateTime.UtcNow;
                foreach (var item in purchaseToSave.Items)
                {
                    var product = await productRepo.GetAsync(item.ProductId);
                    if(product == null) return ResultServiceApplication<PurchaseInvoiceToReturnDTO>
                    .Fail($"Product with ID {item.ProductId} not found");
                    product.Stock += item.Quantity; // update stock in Repo
                    item.ProductName = product.ProductName;
                    item.GetTotalPrice();
                }

                purchaseToSave.GetTotalPurchase();
                await _unitOfWork.CreateRepository<PurchaseInvoice>().AddAsync(purchaseToSave);
                await _unitOfWork.CompleteAsync();

                var purchaseToReturn = _mapper.Map<PurchaseInvoiceToReturnDTO>(purchaseToSave);
                return ResultServiceApplication<PurchaseInvoiceToReturnDTO>.Success(purchaseToReturn, "Purchase report saved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while Adding new purchase invoice");
                return ResultServiceApplication<PurchaseInvoiceToReturnDTO>.Fail("There is a warning in database");
            }
        }
        #endregion

        #region Delete
        public async Task<ResultServiceApplication<bool>> Delete(int id)
        {
            try
            {
                var purchase = await _unitOfWork.CreateRepository<PurchaseInvoice>().GetAsync(id);
                if (purchase is null) return ResultServiceApplication<bool>.Fail("No report Found");
                purchase.IsDeleted = true;
                _unitOfWork.CreateRepository<PurchaseInvoice>().Update(purchase);
                await _unitOfWork.CompleteAsync();
                return ResultServiceApplication<bool>.Success(true, "report deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while deleted purchase invoice");
                return ResultServiceApplication<bool>.Fail("There is a warning in database");
            }
        }
        #endregion

        #region GetPurchaseExport
        public async Task<IReadOnlyList<PurchaseInvoiceExportToReturnDTO>> GetPurchaseExport()
        {
            var spec = new PurchaseSpec();
            var data = await _unitOfWork.CreateRepository<PurchaseInvoice>().GetAllAsyncSpec(spec);
            var result = _mapper.Map<IReadOnlyList<PurchaseInvoiceExportToReturnDTO>>(data);
            return result;
        }

        public async Task<IReadOnlyList<PurchaseInvoiceItemExportToReturnDTO>> GetPurchaseItemsExport()
        {
            var spec = new PurchaseSpec();
            var purchases = await _unitOfWork.CreateRepository<PurchaseInvoice>().GetAllAsyncSpec(spec);

            var result = purchases
                .SelectMany(purchase => purchase.Items.Select(item => new PurchaseInvoiceItemExportToReturnDTO
                {
                    PurchaseInvoiceId = purchase.Id,
                    ItemId = item.Id,
                    UserName = purchase.UserName,
                    CompanyName = purchase.CompanyName,
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

        #region GetPurchaseForImportAsync
        public async Task<ImportToReturnDTO<PurchaseInvoiceToImport>> GetPurchaseForImportAsync(ImportDTO<PurchaseInvoiceToImport> req)
        {
            var importedRows = await _serviceImport.ExcelImportAsync(new ImportDTO<PurchaseInvoiceToImport>
            {
                File = req.File,
                Config = BuildImportConfig<PurchaseInvoiceToImport>("Purchase")
            });

            var importedItems = await _serviceImport.ExcelImportAsync(new ImportDTO<PurchaseInvoiceItemExportToReturnDTO>
            {
                File = req.File,
                Config = BuildImportConfig<PurchaseInvoiceItemExportToReturnDTO>("PurchaseItems")
            });

            var purchaseRepo = _unitOfWork.CreateRepository<PurchaseInvoice>();
            var importedPurchaseIds = importedRows.Data
                .Where(row => row.Id > 0)
                .Select(row => row.Id)
                .Distinct()
                .ToList();

            var existingPurchaseIds = new HashSet<int>();
            if (importedPurchaseIds.Any())
            {
                var existingPurchases = await purchaseRepo.GetAllAsyncSpec(new PurchaseSpec(importedPurchaseIds));
                existingPurchaseIds = existingPurchases
                    .Select(purchase => purchase.Id)
                    .ToHashSet();
            }

            var newRows = importedRows.Data
                .Where(row => row.Id <= 0 || !existingPurchaseIds.Contains(row.Id))
                .ToList();

            var itemLookup = importedItems.Data
                .GroupBy(item => item.PurchaseInvoiceId)
                .ToDictionary(group => group.Key, group => group.ToList());

            var purchasesToSave = newRows.Select(row =>
            {
                var purchase = new PurchaseInvoice
                {
                    UserName = row.UserName,
                    CompanyName = row.CompanyName,
                    CreatedBy = string.IsNullOrWhiteSpace(row.CreatedBy) ? row.UserName : row.CreatedBy,
                    CreatedAt = row.CreatedAt == default ? DateTime.UtcNow : row.CreatedAt
                };

                if (itemLookup.TryGetValue(row.Id, out var purchaseItems))
                {
                    purchase.Items = purchaseItems.Select(item => new PurchaseInvoiceItems
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice
                    }).ToList();

                    purchase.GetTotalPurchase();
                }
                else
                {
                    purchase.TotalReportTransaction = row.TotalReportTransaction;
                }

                return purchase;
            }).ToList();

            if (purchasesToSave.Any())
            {
                await purchaseRepo.AddRangeAsync(purchasesToSave);
                await _unitOfWork.CompleteAsync();
            }

            var errors = (importedRows.Errors ?? new List<string>())
                .Concat(importedItems.Errors ?? new List<string>())
                .Distinct()
                .ToList();
            var skippedPurchasesCount = importedRows.Data.Count - newRows.Count;
            if (skippedPurchasesCount > 0)
            {
                errors.Add($"{skippedPurchasesCount} existing purchase record(s) were skipped during import.");
            }

            return new ImportToReturnDTO<PurchaseInvoiceToImport>
            {
                Data = newRows,
                TotalRows = newRows.Count,
                AddedCount = newRows.Count,
                SkippedDuplicates = skippedPurchasesCount,
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
