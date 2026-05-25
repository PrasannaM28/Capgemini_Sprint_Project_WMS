using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Configurations;

public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
{
    public void Configure(EntityTypeBuilder<Leave> builder)
    {
        builder.ToTable("Leaves");

        builder.HasKey(l => l.LeaveId);

        builder.Property(l => l.LeaveType)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(l => l.Reason)
            .HasMaxLength(255);

        builder.Property(l => l.Status)
            .HasConversion<string>();

        builder.HasOne(l => l.Employee)
            .WithMany(e => e.Leaves)
            .HasForeignKey(l => l.EmpId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
