using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Persistence;

public class ClinicDbContext(DbContextOptions<ClinicDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    public DbSet<Doctor> Doctors => Set<Doctor>();

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<Appointment> Appointments => Set<Appointment>();

    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClinicDbContext).Assembly);
    }
}
