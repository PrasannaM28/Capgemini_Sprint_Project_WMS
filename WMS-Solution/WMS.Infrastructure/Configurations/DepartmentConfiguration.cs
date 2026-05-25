using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.DepartmentId);

        builder.Property(d => d.DepartmentName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(d => d.DepartmentName)
            .IsUnique();

        builder.Property(d => d.Description)
            .HasMaxLength(255);

        builder.Property(d => d.CreatedOn)
            .HasDefaultValueSql("GETUTCDATE()");
    }
}
