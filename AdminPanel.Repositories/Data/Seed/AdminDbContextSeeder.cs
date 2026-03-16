using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.Entities.Products;

namespace AdminPanel.Repositories.Data.Seed
{
    public static class AdminDbContextSeeder
    {
        public static async Task SeederAsync(AdminDbContext dbContext)
        {
            await SeederHelper.SeederFromJSONAsync<Brand>(dbContext, "Brands.Json");
            await SeederHelper.SeederFromJSONAsync<Category>(dbContext, "Categories.Json");
            await SeederHelper.SeederFromJSONAsync<Product>(dbContext, "Products.Json");
        }
    }
}