using AdminPanel.Apis.Helpers.Resolvers;
using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
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
        }
    }
}
