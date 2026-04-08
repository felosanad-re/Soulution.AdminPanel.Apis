using AdminPanel.Core.ModelsDto.RequestDTO.Charts;
using AdminPanel.Core.ModelsDto.ResponseDTO.Charts;
using AdminPanel.Core.Service_Contract.ChartsServices;
using Microsoft.AspNetCore.Mvc;

namespace AdminPanel.Apis.Controllers.Charts
{
    public class ChartController : BaseController
    {
        protected readonly IChartService _chartService;

        public ChartController(IChartService chartService)
        {
            _chartService = chartService;
        }

        [HttpGet("Charts")] // Get: /api/Chart/Charts
        public async Task<ActionResult<ChartsToReturnDTO>> GetChart([FromQuery]ChartsDTO dTO)
        {
            var data = await _chartService.GetCharts(dTO);
            return Ok(data);
        }
    }
}
