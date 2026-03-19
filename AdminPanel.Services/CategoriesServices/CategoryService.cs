using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.ModelsDto.RequestDTO;
using AdminPanel.Core.ModelsDto.RequestDTO.Categories;
using AdminPanel.Core.ModelsDto.ResponseDTO.Categories;
using AdminPanel.Core.Service_Contract.AttachmentServices;
using AdminPanel.Core.Service_Contract.CategoriesServices;
using AdminPanel.Core.UnitOfWork;
using AdminPanel.Services.BrandsServices;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Services.CategoriesServices
{
    public class CategoryService : ICategoryService
    {
        #region Services
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly IAttachmentService _attachmentService;
        protected readonly ILogger<BrandService> _logger;
        protected readonly IConfiguration _configuration;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService, ILogger<BrandService> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
            _logger = logger;
            _configuration = configuration;
        }
        #endregion

        public async Task<ResultServiceApplication<IReadOnlyList<CategoryToReturnDTO>>> GetAllAsync()
        {
            try
            {
                var result = await _unitOfWork.CreateRepository<Category>().GetAllAsync();
                if (!result.Any()) return ResultServiceApplication<IReadOnlyList<CategoryToReturnDTO>>.Fail("there is no categories to show");

                return ResultServiceApplication<IReadOnlyList<CategoryToReturnDTO>>.Success(_mapper.Map<IReadOnlyList<CategoryToReturnDTO>>(result), "this is all categories");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<IReadOnlyList<CategoryToReturnDTO>>.Fail(ex.Message);
            }
        }

        public async Task<ResultServiceApplication<CategoryToReturnDTO>> GetCategoryDetailsAsync(int id)
        {
            try
            {
                var result = await _unitOfWork.CreateRepository<Category>().GetAsync(id);
                if (result is null) return ResultServiceApplication<CategoryToReturnDTO>.Fail("there is no category found");

                return ResultServiceApplication<CategoryToReturnDTO>.Success(_mapper.Map<CategoryToReturnDTO>(result), "show category succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<CategoryToReturnDTO>.Fail(ex.Message);
            }
        }

        #region Add Category Async
        public async Task<ResultServiceApplication<CategoryToReturnDTO>> AddCategoryAsync(CreatedCategoryDTO dTO)
        {
            try
            {
                var result = new Category()
                {
                    CategoryName = dTO.CategoryName,
                    Description = dTO.Description,
                };
                if (dTO.Image != null)
                {
                    var allowedExtentions = _configuration.GetSection("FileSitteng:Allowed_Extentions").Get<string[]>();
                    var maxSize = int.Parse(_configuration["FileSitteng:MaxSize"]!);
                    result.Image = await _attachmentService.UploadAsync(dTO.Image,
                        _configuration["FileSitteng:CategoryImages"]!,
                        allowedExtentions!,
                        maxSize);
                }
                await _unitOfWork.CreateRepository<Category>().AddAsync(result);
                await _unitOfWork.CompleteAsync();
                return ResultServiceApplication<CategoryToReturnDTO>.Success(_mapper.Map<CategoryToReturnDTO>(result), "category added succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<CategoryToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Updated CategoryAsync
        public async Task<ResultServiceApplication<CategoryToReturnDTO>> UpdatedCategoryAsync(UpdatedCategoryDTO dTO)
        {
            try
            {
                var result = await _unitOfWork.CreateRepository<Category>().GetAsync(dTO.Id);
                if (result is null) return ResultServiceApplication<CategoryToReturnDTO>.Fail("there is no category found");
                result.CategoryName = dTO.CategoryName;
                result.Description = dTO.Description;
                if (dTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(result.Image))
                    {
                        await _attachmentService.DeleteImageAsync(result.Image, _configuration["FileSitteng:CategoryImages"]);
                    }
                    var allowedExtenstions = _configuration.GetSection("FileSitteng:Allowed_Extentions").Get<string[]>();
                    var maxSize = int.Parse(_configuration["FileSitteng:MaxSize"]!);
                    var folderName = _configuration["FileSitteng:CategoryImages"];
                    result.Image = await _attachmentService.UploadAsync(
                        dTO.Image,
                        folderName,
                        allowedExtenstions,
                        maxSize);
                }
                _unitOfWork.CreateRepository<Category>().Update(result);
                await _unitOfWork.CompleteAsync();
                var data = _mapper.Map<CategoryToReturnDTO>(result);
                return ResultServiceApplication<CategoryToReturnDTO>.Success(data, "Category updated succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<CategoryToReturnDTO>.Fail(ex.Message);
            }
        }
        #endregion

        #region Deleted Category
        public async Task<ResultServiceApplication<bool>> DeletedCategory(int id)
        {
            try
            {
                var result = await _unitOfWork.CreateRepository<Category>().GetAsync(id);
                if (result == null) return ResultServiceApplication<bool>.Fail("no category found");

                result.IsDeleted = true;
                _unitOfWork.CreateRepository<Category>().Update(result);
                await _unitOfWork.CompleteAsync();
                return ResultServiceApplication<bool>.Success(true, "category deleted succeeded");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResultServiceApplication<bool>.Fail(ex.Message);
            }
        }
        #endregion
    }
}
