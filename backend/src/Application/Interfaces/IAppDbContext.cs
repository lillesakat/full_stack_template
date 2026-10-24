using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces;

public interface IAppDbContext
{
    DbSet<Period> Periods { get; }
    DbSet<Department> Departments { get; }
    DbSet<StaffGroup> StaffGroups { get; }
    DbSet<User> Users { get; }
    DbSet<Employment> Employments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
