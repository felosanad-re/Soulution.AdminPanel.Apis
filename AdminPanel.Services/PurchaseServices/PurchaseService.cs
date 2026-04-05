using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Purchases;
using AdminPanel.Core.ModelsDto.ResponseDTO.Purchases;
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
        public PurchaseService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PurchaseService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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
                var createPurchase = _mapper.Map<PurchaseInvoice>(dto);
                createPurchase.CreatedBy = userName; // Admin Account
                createPurchase.UserName = userName; // Admin Name
                createPurchase.CreatedAt = DateTime.UtcNow;
                foreach (var item in createPurchase.Items)
                {
                    var product = await productRepo.GetAsync(item.ProductId);
                    if(product == null) return ResultServiceApplication<PurchaseInvoiceToReturnDTO>
                    .Fail($"Product with ID {item.ProductId} not found");
                    product.Stock += item.Quantity; // update stock in Repo
                    item.ProductName = product.ProductName;
                    item.GetTotalPrice();
                }
                createPurchase.GetTotalPurchase();
                await _unitOfWork.CreateRepository<PurchaseInvoice>().AddAsync(createPurchase);
                await _unitOfWork.CompleteAsync();
                var data = _mapper.Map<PurchaseInvoiceToReturnDTO>(createPurchase);
                return ResultServiceApplication<PurchaseInvoiceToReturnDTO>.Success(data, "Purchase Report Save Successfully");
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
    }
}
