using AdminPanel.Core.ModelsDto.RequestDTO.Charts;
using AdminPanel.Core.ModelsDto.ResponseDTO.Charts;

namespace AdminPanel.Core.Service_Contract.ChartsServices
{
    public interface IChartService
    {
        Task<ChartsToReturnDTO> GetCharts(ChartsDTO dTO);
    }
}
