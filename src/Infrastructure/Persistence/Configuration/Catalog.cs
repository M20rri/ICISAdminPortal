using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mukesh.Domain.Catalog;

namespace Mukesh.Infrastructure.Persistence.Configuration;
public class BrandConfig : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.IsMultiTenant();

        builder
            .Property(b => b.NameAr)
                .HasMaxLength(256)
                .IsRequired();

        builder
            .Property(b => b.NameEn)
            .HasColumnType("varchar(256)")
            .IsRequired(false);

    }
}

public class ModelConfig : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.IsMultiTenant();

        builder
            .Property(b => b.NameAr)
            .HasMaxLength(256);

        builder
            .Property(b => b.NameEn)
            .HasColumnType("varchar(256)")
            .IsRequired(false);
    }

}

public class ActionPageConfig : IEntityTypeConfiguration<ActionPage>
{
    public void Configure(EntityTypeBuilder<ActionPage> builder)
    {
        builder.IsMultiTenant();
    }

}

public class PageConfig : IEntityTypeConfiguration<Page>
{
    public void Configure(EntityTypeBuilder<Page> builder)
    {
        builder.IsMultiTenant();
    }

}

public class ModuleConfig : IEntityTypeConfiguration<Module>
{
    public void Configure(EntityTypeBuilder<Module> builder)
    {
        builder.IsMultiTenant();
    }

}

public class PermissionConfig : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.IsMultiTenant();
    }

}
