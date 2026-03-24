using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
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

        public ReportTransactionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ReportTransactionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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
                var newReport = _mapper.Map<CreateReportDTO, ReportTransaction>(dTO); // Reverse Mapping
                newReport.CreatedBy = userId;
                newReport.CreatedAt = DateTime.UtcNow;
                await _unitOfWork.CreateRepository<ReportTransaction>().AddAsync(newReport);
                await _unitOfWork.CompleteAsync();
                var result = _mapper.Map<ReportTransaction, ReportTransactionToReturnDTO>(newReport);
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

        #region Update Report Async
        public async Task<ResultServiceApplication<ReportTransactionToReturnDTO>> UpdateReportAsync(string userId, UpdateReportDTO dTO)
        {
            try
            {
                var report = await _unitOfWork.CreateRepository<ReportTransaction>().GetAsync(dTO.Id);
                if (report is null) return ResultServiceApplication<ReportTransactionToReturnDTO>.Fail("Report Not Found");

                _mapper.Map(dTO, report); // Update
                report.ModifiedBy = userId;
                report.LastModifiedAt = DateTime.UtcNow;
                _unitOfWork.CreateRepository<ReportTransaction>().Update(report);
                await _unitOfWork.CompleteAsync();
                var result = _mapper.Map<ReportTransactionToReturnDTO>(report);
                return ResultServiceApplication<ReportTransactionToReturnDTO>.Success(result, "Report Updated Succeeded");
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
    }
}
