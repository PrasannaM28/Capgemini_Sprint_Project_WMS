using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Configurations;

public class EmployeeProjectAllocationConfiguration
    : IEntityTypeConfiguration<EmployeeProjectAllocation>
{
    public void Configure(EntityTypeBuilder<EmployeeProjectAllocation> builder)
    {
        builder.ToTable("EmployeeProjectAllocations");

        builder.HasKey(a => a.AllocationId);

        builder.Property(a => a.CreatedBy)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(50);

        builder.Property(a => a.Status)
            .HasDefaultValue(true);

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.Allocations)
            .HasForeignKey(a => a.EmpId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Project)
            .WithMany(p => p.Allocations)
            .HasForeignKey(a => a.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => new
        {
            a.EmpId,
            a.ProjectId
        }).IsUnique();
    }
}
