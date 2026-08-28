using EventService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EventService.Infrastructure.Persistence.Configurations;

public sealed class ZoneConfiguration : IEntityTypeConfiguration<Zone>
{
    public void Configure(EntityTypeBuilder<Zone> builder)
    {
        builder.ToTable("Zones");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(z => z.Price)
            .IsRequired()
            .HasColumnType("numeric(12,2)");

        builder.Property(z => z.Capacity)
            .IsRequired();

        builder.Property(z => z.EventId)
            .IsRequired();
    }
}
