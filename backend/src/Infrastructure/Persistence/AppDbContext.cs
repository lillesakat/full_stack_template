using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Period> Periods => Set<Period>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<StaffGroup> StaffGroups => Set<StaffGroup>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Employment> Employments => Set<Employment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}
