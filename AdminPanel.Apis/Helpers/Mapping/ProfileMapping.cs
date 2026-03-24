using AdminPanel.Apis.Helpers.Resolvers;
using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO.Brands;
using AdminPanel.Core.ModelsDto.RequestDTO.Categories;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Brands;
using AdminPanel.Core.ModelsDto.ResponseDTO.Categories;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AutoMapper;

namespace AdminPanel.Apis.Helpers.Mapping
{
    public class ProfileMapping: Profile
    {
        public ProfileMapping()
        {
            // Start mapping
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand!.BrandName))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category!.CategoryName))
                .ForMember(d => d.MainImage, o => o.MapFrom<ImageUrlResolver<Product, ProductToReturnDTO>, string>(s => $"Files/Images/Products/main/{s.MainImage}"));

            CreateMap<Product, CreateProductDTO>();
            CreateMap<Product, UpdateProductDTO>();
            CreateMap<ProductImages, ProductImagesDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.ImagesUrl, o => o.MapFrom<ImageUrlResolver<ProductImages, ProductImagesDto>, string>(s =>$"Files/Images/Products/Sub/{s.ImagesUrl}"));

            CreateMap<Brand, BrandToReturnDTO>()
                .ForMember(d => d.Logo, o => o.MapFrom<ImageUrlResolver<Brand, BrandToReturnDTO>, string>(s => $"Files/Images/Brands/{s.Logo}"));
            CreateMap<Brand, CreatedBrandDTO>();
            CreateMap<Brand, UpdatedBrandDTO>();

            CreateMap<Category, CategoryToReturnDTO>()
                .ForMember(d => d.Image, o => o.MapFrom<ImageUrlResolver<Category, CategoryToReturnDTO>, string> (s => $"Files/Images/categories/{s.Image}"));
            CreateMap<Category, CreatedCategoryDTO>();
            CreateMap<Category, UpdatedCategoryDTO>();

            CreateMap<ReportTransaction, ReportTransactionToReturnDTO>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.ApplicationUser.Id))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.ApplicationUser.UserName))
                .ForMember(d => d.ModifiedBy, o => o.MapFrom(s => s.ApplicationUser.UserName));

            CreateMap<ReportTransactionItem, ReportTransactionItemDTO>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.Product.Id));

            CreateMap<ReportTransaction, CreateReportDTO>().ReverseMap();
            CreateMap<ReportTransaction, UpdateReportDTO>().ReverseMap();
            CreateMap<ReportTransactionItem, ReportTransactionItemToReturnDTO>();
        }
    }
}
