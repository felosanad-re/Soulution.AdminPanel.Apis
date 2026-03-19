using AdminPanel.Apis.Helpers.Resolvers;
using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.ModelsDto.RequestDTO.Brands;
using AdminPanel.Core.ModelsDto.RequestDTO.Categories;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Brands;
using AdminPanel.Core.ModelsDto.ResponseDTO.Categories;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
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
                .ForMember(d => d.MainImage, o => o.MapFrom<ImageUrlResolver<Product, ProductToReturnDTO>, string>(s => s.MainImage));

            CreateMap<Product, CreateProductDTO>();
            CreateMap<Product, UpdateProductDTO>();
            CreateMap<ProductImages, ProductImagesDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.ImagesUrl, o => o.MapFrom<ImageUrlResolver<ProductImages, ProductImagesDto>, string>(s => s.ImagesUrl));

            CreateMap<Brand, BrandToReturnDTO>()
                .ForMember(d => d.Logo, o => o.MapFrom<ImageUrlResolver<Brand, BrandToReturnDTO>, string>(s => s.Logo));
            CreateMap<Brand, CreatedBrandDTO>();
            CreateMap<Brand, UpdatedBrandDTO>();

            CreateMap<Category, CategoryToReturnDTO>()
                .ForMember(d => d.Image, o => o.MapFrom<ImageUrlResolver<Category, CategoryToReturnDTO>, string> (s => s.Image));
            CreateMap<Category, CreatedCategoryDTO>();
            CreateMap<Category, UpdatedCategoryDTO>();
        }
    }
}
