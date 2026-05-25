using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Configurations;

public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => a.AttendanceId);

        builder.Property(a => a.WorkMode)
            .HasConversion<string>();

        builder.Property(a => a.TotalHours)
            .HasPrecision(5, 2);

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EmpId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
