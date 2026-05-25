using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WMS.Domain.Entities;

namespace WMS.Infrastructure.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(c => c.ClientId);

        builder.Property(c => c.ClientName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.ClientPhoneNumber)
            .HasMaxLength(10);

        builder.Property(c => c.ClientLocation)
            .HasMaxLength(20);

        builder.Property(c => c.Status)
            .HasDefaultValue(true);
    }
}
