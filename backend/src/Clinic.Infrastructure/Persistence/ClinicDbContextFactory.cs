using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Clinic.Infrastructure.Persistence;

public class ClinicDbContextFactory : IDesignTimeDbContextFactory<ClinicDbContext>
{
    public ClinicDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ClinicDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=clinicdb;Username=postgres;Password=91322")
            .Options;

        return new ClinicDbContext(options);
    }
}
