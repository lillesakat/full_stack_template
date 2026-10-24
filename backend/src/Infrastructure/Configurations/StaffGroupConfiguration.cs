using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Configurations;

public class StaffGroupConfiguration : IEntityTypeConfiguration<StaffGroup>
{
    public void Configure(EntityTypeBuilder<StaffGroup> builder)
    {
        builder.Property(s => s.Name).IsRequired().HasMaxLength(100);
    }
}
