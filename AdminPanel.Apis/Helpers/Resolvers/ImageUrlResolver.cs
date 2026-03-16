using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.ModelsDto.ResponseDTO.Products;
using AutoMapper;

namespace AdminPanel.Apis.Helpers.Resolvers
{
    public class ImageUrlResolver<TSource, TDestination> : IMemberValueResolver<TSource, TDestination, string, string>
    {
        private readonly IConfiguration _config;

        public ImageUrlResolver(IConfiguration configuration)
        {
            _config = configuration;
        }

        public string Resolve(TSource source, TDestination destination, string sourceMember, string destMember, ResolutionContext context)
        {
            if (!string.IsNullOrEmpty(sourceMember))
                return $"{_config["BasePictureUrl"]}/{sourceMember}";
            return string.Empty;
        }
    }
}
