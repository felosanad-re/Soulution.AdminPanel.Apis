using AdminPanel.Core.Entities.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPanel.Repositories.Data.Configurations.CategoriesConfigurations
{
    public class CategoryConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(C => C.CreatedAt).HasColumnType("datetime2(0)");
            builder.Property(C => C.LastModifiedAt).HasColumnType("datetime2(0)");
        }
    }
}
