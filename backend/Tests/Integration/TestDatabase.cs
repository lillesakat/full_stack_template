using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Tests.Integration;

// En ny, tom SQLite-database i hukommelsen. Databasen findes kun, så længe forbindelsen er åben.
public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public AppDbContext Context { get; }

    public TestDatabase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new AppDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }

    // Opretter en ansættelse med sin egen bruger, sit eget afsnit og sin egen personalegruppe.
    public async Task<Employment> AddEmploymentAsync()
    {
        var employment = new Employment
        {
            User = new User { Name = "Test Person", Email = $"{Guid.NewGuid()}@test.dk", IsStaff = true },
            Department = new Department { Name = "Testafsnit", Color = "#123456" },
            StaffGroup = new StaffGroup { Name = "Testgruppe" },
            HoursPerWeek = 37,
            StartDate = new DateOnly(2026, 1, 1),
            EndDate = new DateOnly(2026, 12, 31)
        };

        Context.Employments.Add(employment);
        await Context.SaveChangesAsync();

        return employment;
    }
}