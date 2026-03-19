using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Brands;
using AdminPanel.Core.ModelsDto.ResponseDTO.Brands;
using AdminPanel.Core.Service_Contract.AttachmentServices;
using AdminPanel.Core.Service_Contract.brandsServices;
using AdminPanel.Core.UnitOfWork;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;

namespace AdminPanel.Services.BrandsServices
{
    public class BrandService : IBrandService
    {
        #region Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly IAttachmentService _attachmentService;
        protected readonly ILogger<BrandService> _logger;
        protected readonly IConfiguration _configuration;

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService, ILogger<BrandService> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        #region Get Brands Async
        public async Task<ResultServiceApplication<IReadOnlyList<BrandToReturnDTO>>> GetBrandsAsync()
        {
            try
            {
                var result = await _unitOfWork.CreateRepository<Brand>().GetAllAsync();
                if (!result.Any()) return ResultServiceApplication<IReadOnlyList<BrandToReturnDTO>>.Fail("There is No Brands To show");

                return ResultServiceApplication<IReadOnlyList<BrandToReturnDTO>>.Success(_mapper.Map<IReadOnlyList<BrandToReturnDTO>>(result), "There is brands to show");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<IReadOnlyList<BrandToReturnDTO>>.Fail(ex.Message);
            }
        }
        #endregion

        #region Get Brand Details Async
        public async Task<ResultServiceApplication<BrandToReturnDTO>> GetBrandDetailsAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.CreateRepository<Brand>().GetAsync(id);
                if (result == null) return ResultServiceApplication<BrandToReturnDTO>.Fail("there is no brand with this id");

                return ResultServiceApplication<BrandToReturnDTO>.Success(_mapper.Map<BrandToReturnDTO>(result), "show succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<BrandToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Add Brand Async
        public async Task<ResultServiceApplication<BrandToReturnDTO>> AddBrandAsync(CreatedBrandDTO brand)
        {
            try
            {
                var allowExtentions = _configuration.GetSection("FileSitteng:Allowed_Extentions").Get<string[]>();
                var maxSize = _configuration.GetValue<int>("FileSitteng:MaxSize");
                var newBrand = new Brand()
                {
                    BrandName = brand.BrandName,
                    Description = brand.Description,
                };
                if (brand.Logo != null)
                {
                    var folderName = _configuration["FileSitteng:BrandImages"];
                    newBrand.Logo = await _attachmentService.UploadAsync(brand.Logo,
                        folderName,
                        allowExtentions,
                        maxSize);
                }

                await _unitOfWork.CreateRepository<Brand>().AddAsync(newBrand);
                await _unitOfWork.CompleteAsync();

                return ResultServiceApplication<BrandToReturnDTO>.Success(_mapper.Map<BrandToReturnDTO>(newBrand), "Brand created succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<BrandToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Update Brand Async
        public async Task<ResultServiceApplication<BrandToReturnDTO>> UpdateBrandAsync(UpdatedBrandDTO brand)
        {
            try
            {
                var allowExtentions = _configuration.GetSection("FileSitteng:Allowed_Extentions").Get<string[]>();
                var maxSize = _configuration.GetValue<int>("FileSitteng:MaxSize");
                var updatedBrand = await _unitOfWork.CreateRepository<Brand>().GetAsync(brand.Id);
                if (updatedBrand == null) return ResultServiceApplication<BrandToReturnDTO>.Fail("there is no brand with this id");

                updatedBrand.BrandName = brand.BrandName;
                updatedBrand.Description = brand.Description;

                if (brand.Logo != null)
                {
                    if(!string.IsNullOrEmpty(updatedBrand.Logo))
                    {
                        await _attachmentService.DeleteImageAsync(updatedBrand.Logo, _configuration["FileSitteng:BrandImages"]!);
                    }
                    var folderName = "Images/Brands";
                    updatedBrand.Logo = await _attachmentService.UploadAsync(brand.Logo, folderName, allowExtentions, maxSize);
                }
                _unitOfWork.CreateRepository<Brand>().Update(updatedBrand);
                await _unitOfWork.CompleteAsync();

                return ResultServiceApplication<BrandToReturnDTO>.Success(_mapper.Map<BrandToReturnDTO>(updatedBrand), "Brand updated succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<BrandToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Delete Brand Async
        public async Task<ResultServiceApplication<bool>> DeleteBrandAsync(int id)
        {
            var result = await _unitOfWork.CreateRepository<Brand>().GetAsync(id);
            if (result == null) return ResultServiceApplication<bool>.Fail("there is no brand with this id");
            result.IsDeleted = true;
            await _unitOfWork.CompleteAsync();

            return ResultServiceApplication<bool>.Success(true, "brand deleted succeeded");
        }
        #endregion

    }
}
