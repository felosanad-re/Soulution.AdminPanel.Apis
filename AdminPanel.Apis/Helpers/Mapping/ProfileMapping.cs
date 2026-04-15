using AdminPanel.Apis.Helpers.Resolvers;
using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.Entities.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO.Brands;
using AdminPanel.Core.ModelsDto.RequestDTO.Categories;
using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using AdminPanel.Core.ModelsDto.RequestDTO.Purchases;
using AdminPanel.Core.ModelsDto.RequestDTO.Reports;
using AdminPanel.Core.ModelsDto.RequestDTO.Roles;
using AdminPanel.Core.ModelsDto.ResponseDTO.Brands;
using AdminPanel.Core.ModelsDto.ResponseDTO.Categories;
using AdminPanel.Core.ModelsDto.ResponseDTO.Imports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Purchases;
using AdminPanel.Core.ModelsDto.ResponseDTO.Reports;
using AdminPanel.Core.ModelsDto.ResponseDTO.Roles;
using AdminPanel.Core.ModelsDto.ResponseDTO.User;
using AdminPanel.Core.ModelsDTO.ResponseDTO;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AdminPanel.Apis.Helpers.Mapping
{
    public class ProfileMapping: Profile
    {
        public ProfileMapping()
        {
            // Start mapping
            #region Product & Category & Brand
            CreateMap<Product, ProductToReturnDTO>()
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand!.BrandName))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category!.CategoryName))
                .ForMember(d => d.MainImage, o => o.MapFrom<ImageUrlResolver<Product, ProductToReturnDTO>, string>(s => $"Files/Images/Products/main/{s.MainImage}"));

            CreateMap<Product, CreateProductDTO>();
            CreateMap<Product, UpdateProductDTO>();
            CreateMap<ProductImages, ProductImagesDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.ImagesUrl, o => o.MapFrom<ImageUrlResolver<ProductImages, ProductImagesDto>, string>(s => $"Files/Images/Products/Sub/{s.ImagesUrl}"));

            CreateMap<Brand, BrandToReturnDTO>()
                .ForMember(d => d.Logo, o => o.MapFrom<ImageUrlResolver<Brand, BrandToReturnDTO>, string>(s => $"Files/Images/Brands/{s.Logo}"));
            CreateMap<Brand, CreatedBrandDTO>();
            CreateMap<Brand, UpdatedBrandDTO>();

            CreateMap<Category, CategoryToReturnDTO>()
                .ForMember(d => d.Image, o => o.MapFrom<ImageUrlResolver<Category, CategoryToReturnDTO>, string>(s => $"Files/Images/categories/{s.Image}"));
            CreateMap<Category, CreatedCategoryDTO>();
            CreateMap<Category, UpdatedCategoryDTO>();

            // Export
            CreateMap<Product, ProductExportToReturnDTO>()
                .ForMember(dest => dest.SubImages, opt => opt.MapFrom(src =>
                src.SubImages != null
                    ? string.Join(" And ", src.SubImages
                        .Where(img => !string.IsNullOrWhiteSpace(img.ImagesUrl))
                        .Select(img => img.ImagesUrl))
                : ""))
                .ForMember(d => d.BrandName, o => o.MapFrom(s => s.Brand!.BrandName))
                .ForMember(d => d.CategoryName, o => o.MapFrom(s => s.Category!.CategoryName))
                .ForMember(d => d.MainImage, o => o.MapFrom(s => s.MainImage));

            // Import 
            CreateMap<ProductToImport, Product>()
                .ForMember(d => d.SubImages, o => o.Ignore())
                .ForMember(d => d.BrandId, o => o.MapFrom(s => s.BrandId))
                .ForMember(d => d.CategoryId, o => o.MapFrom(s => s.CategoryId))
                .ForMember(d => d.Brand, o => o.Ignore())
                .ForMember(d => d.Category, o => o.Ignore());
            #endregion

            #region Report Mapping
            CreateMap<ReportTransaction, SalesReportTransactionToReturnDTO>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.UserName : s.CreatedBy))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.Id : s.UserId))
                .ForMember(d => d.CreatedBy, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.UserName : s.CreatedBy))
                .ForMember(d => d.ModifiedBy, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.UserName : s.ModifiedBy))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            CreateMap<SalesReportTransactionItemDTO, ReportTransactionItem>();

            CreateMap<CreateSalesReportDTO, ReportTransaction>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));

            CreateMap<ReportTransactionItem, SalesReportTransactionItemToReturnDTO>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
                .ForMember(d => d.Price, o => o.MapFrom(s => s.Price))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ProductName));

            // For Export
            CreateMap<ReportTransaction, SalesReportTransactionExportToReturnDTO>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items !=null ? string.Join(" And ", s.Items.Select(x => x.ProductName)): ""))
                .ForMember(d => d.TotalReportTransactionPrice, o => o.MapFrom(s => s.TotalReportTransaction))
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.ApplicationUser != null ? s.ApplicationUser.UserName : s.CreatedBy));
            #endregion

            #region PurchaseInvoice Mapping
            CreateMap<PurchaseInvoice, PurchaseInvoiceToReturnDTO>()
                .ForMember(d => d.UserName, o => o.MapFrom(s => s.UserName))
                .ForMember(d => d.TotalReportTransaction, o => o.MapFrom(s => s.TotalReportTransaction))
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items));
            CreateMap<PurchaseInvoiceItems, PurchaseInvoiceItemsToReturnDTO>();
            CreateMap<CreatePurchaseDTO, PurchaseInvoice>();
            CreateMap<PurchaseInvoiceItemsDTO, PurchaseInvoiceItems>();

            // For Export
            CreateMap<PurchaseInvoice, PurchaseInvoiceExportToReturnDTO>()
                .ForMember(d => d.Items, o => o.MapFrom(s => s.Items != null ? string.Join(" And ", s.Items.Select(x => x.ProductName)): ""));
            #endregion

            #region User Mapping Roles
            CreateMap<ApplicationUser, ApplicationUserToReturnDTO>();
            CreateMap<ApplicationUser, CreateToReturnDTO>();
            CreateMap<IdentityRole, RoleToReturnDTO>();
            CreateMap<CreatedRoleDTO, IdentityRole>();
            CreateMap<UpdatedRoleDTO, IdentityRole>();
            #endregion
        }
    }
}
