using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Purchases;
using AdminPanel.Core.ModelsDto.ResponseDTO.Purchases;
using AdminPanel.Core.Service_Contract.PurchaseServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace AdminPanel.Apis.Controllers.Purchase
{
    [Authorize]
    public class PurchaseController : BaseController
    {
        #region Services
        protected readonly IPurchaseService _purchaseService;

        public PurchaseController(IPurchaseService purchaseService)
        {
            _purchaseService = purchaseService;
        }
        #endregion

        #region Get All
        [HttpGet("Purchases")] // GET: /api/Purchase/Purchases
        public async Task<ActionResult<ResultServiceApplication<IReadOnlyList<PurchaseInvoiceToReturnDTO>>>> GetAll()
        {
            var data = await _purchaseService.GetAllAsync();
            return Ok(data);
        }
        #endregion

        #region Get Details
        [HttpGet("{id}")] // GET: /api/Purchase/id
        public async Task<ActionResult<ResultServiceApplication<PurchaseInvoiceToReturnDTO>>> Get(int id)
        {
            var data = await _purchaseService.GetAsync(id);
            return Ok(data);
        }
        #endregion

        #region Add New Purchase
        [HttpPost("AddPurchase")] // Post: /api/Purchase/AddPurchase
        public async Task<ActionResult<ResultServiceApplication<PurchaseInvoiceToReturnDTO>>> Add(CreatePurchaseDTO dTO)
        {
            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            var data = await _purchaseService.AddPurchaseAsync(dTO, userName!);
            return Ok(data);
        }
        #endregion

        #region Delete Purchase
        [HttpDelete("DeletePurchase/{id}")] // Delete: /api/Purchase/DeletePurchase/id
        public async Task<ActionResult<ResultServiceApplication<bool>>> Delete(int id)
        {
            var result = await _purchaseService.Delete(id);
            return Ok(result);
        }
        #endregion
    }
}
