using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AdminPanel.Core.Service_Contract.ReportServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdminPanel.Apis.Controllers.Reports
{
    [Authorize]
    public class SalesReportController : BaseController
    {
        #region Services
        protected readonly ISalesReportTransactionService _salesReportTransactionService;

        public SalesReportController(ISalesReportTransactionService salesReportTransactionService)
        {
            _salesReportTransactionService = salesReportTransactionService;
        }
        #endregion

        #region Get All
        [HttpGet("salesReports")] // Get: /api/SalesReport/salesReports
        public async Task<ActionResult<ResultServiceApplication<IReadOnlyList<SalesReportTransactionToReturnDTO>>>> GetAll()
        {
            var data = await _salesReportTransactionService.GetAllAsync();
            return Ok(data);
        }
        #endregion

        #region Get Report By Id
        [HttpGet("details/{id}")] // Get: /api/SalesReport/details/id
        public async Task<ActionResult<ResultServiceApplication<SalesReportTransactionToReturnDTO>>> Get(int id)
        {
            var result = await _salesReportTransactionService.GetDetailsSalesReportAsync(id);
            return Ok(result);
        }
        #endregion

        #region Add Report
        [HttpPost("add")] // Post: /api/SalesReport/add
        public async Task<ActionResult<ResultServiceApplication<SalesReportTransactionToReturnDTO>>> Add([FromBody] CreateSalesReportDTO dto)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(user is null) return Unauthorized();
            var result = await _salesReportTransactionService.AddSalesReportAsync(user, dto);
            return Ok(result);
        }
        #endregion

        #region Delete Report
        [HttpDelete("delete/{id}")] // Delete: /api/SalesReport/delete/id
        public async Task<ActionResult<ResultServiceApplication<bool>>> Delete(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            var result = await _salesReportTransactionService.DeleteSalesReportAsync(id);
            return Ok(new ResultServiceApplication<bool>
            {
                Succeed = true,
                Message = $"{result.Message} deleted by: {userName}"
            });
        }
        #endregion
    }
}
