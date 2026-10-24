using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class PeriodConfiguration : IEntityTypeConfiguration<Period>
{
    public void Configure(EntityTypeBuilder<Period> builder)
    {
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.ToTable(t => t.HasCheckConstraint("CK_Period_StartBeforeEnd", "StartDate < EndDate"));
    }
}
