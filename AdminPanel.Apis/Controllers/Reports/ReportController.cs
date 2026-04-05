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
    public class ReportController : BaseController
    {
        #region Services
        protected readonly IReportTransactionService _reportTransactionService;

        public ReportController(IReportTransactionService reportTransactionService)
        {
            _reportTransactionService = reportTransactionService;
        }
        #endregion

        #region Get All
        [HttpGet("reports")] // Get: /api/report/reports
        public async Task<ActionResult<ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>>> GetAll()
        {
            var data = await _reportTransactionService.GetAllAsync();
            //if (data is null) return BadRequest(ResultServiceApplication<IReadOnlyList<ReportTransactionToReturnDTO>>.Fail(""));
            return Ok(data);
        }
        #endregion

        #region Get Report By Id
        [HttpGet("reportDetails/{id}")] // Get: /api/report/reportDetails/id
        public async Task<ActionResult<ResultServiceApplication<ReportTransactionToReturnDTO>>> Get(int id)
        {
            var result = await _reportTransactionService.GetDetailsReportAsync(id);
            return Ok(result);
        }
        #endregion

        #region Add Report
        [HttpPost("AddReport")] // Post: /api/report/AddReport
        public async Task<ActionResult<ResultServiceApplication<ReportTransactionToReturnDTO>>> Add([FromBody] CreateReportDTO dTO)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(user is null) return Unauthorized();
            var result = await _reportTransactionService.AddReportAsync(user, dTO);
            return Ok(result);
        }
        #endregion

        #region Delete Report
        [HttpDelete("DeleteReport/{id}")] // Delete: /api/report/deleteReport/id
        public async Task<ActionResult<ResultServiceApplication<bool>>> Delete(int id)
        {
            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            var result = await _reportTransactionService.DeleteReportAsync(id);
            return Ok(new ResultServiceApplication<bool>
            {
                Succeed = true,
                Message = $"{result.Message} deleted by: {userName}"
            });
        }
        #endregion
    }
}
