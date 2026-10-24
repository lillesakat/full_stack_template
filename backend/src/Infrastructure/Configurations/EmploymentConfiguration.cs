using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class EmploymentConfiguration : IEntityTypeConfiguration<Employment>
{
    public void Configure(EntityTypeBuilder<Employment> builder)
    {
        builder.HasOne(e => e.User)
            .WithMany(u => u.Employments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employments)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.StaffGroup)
            .WithMany(s => s.Employments)
            .HasForeignKey(e => e.StaffGroupId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Employment_HoursPerWeek", "HoursPerWeek BETWEEN 0 AND 37");
            t.HasCheckConstraint("CK_Employment_StartBeforeEnd", "StartDate < EndDate");
        });
    }
}


