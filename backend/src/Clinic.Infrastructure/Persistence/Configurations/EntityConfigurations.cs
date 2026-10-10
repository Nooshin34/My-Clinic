using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("patients");
        builder.HasKey(patient => patient.Id);
        builder.Property(patient => patient.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(patient => patient.LastName).HasMaxLength(80).IsRequired();
        builder.Property(patient => patient.NationalId).HasMaxLength(10).IsRequired();
        builder.HasIndex(patient => patient.NationalId).IsUnique();
        builder.Property(patient => patient.Phone).HasMaxLength(20).IsRequired();
        builder.Property(patient => patient.Address).HasMaxLength(300);
        builder.Property(patient => patient.Gender).HasConversion<string>().HasMaxLength(16);
    }
}

public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
{
    public void Configure(EntityTypeBuilder<Doctor> builder)
    {
        builder.ToTable("doctors");
        builder.HasKey(doctor => doctor.Id);
        builder.Property(doctor => doctor.FirstName).HasMaxLength(80).IsRequired();
        builder.Property(doctor => doctor.LastName).HasMaxLength(80).IsRequired();
        builder.Property(doctor => doctor.Specialty).HasMaxLength(80).IsRequired();
        builder.Property(doctor => doctor.Phone).HasMaxLength(20).IsRequired();
        builder.Property(doctor => doctor.Email).HasMaxLength(120).IsRequired();
        builder.HasIndex(doctor => doctor.Email).IsUnique();
        builder.Property(doctor => doctor.LicenseNumber).HasMaxLength(32).IsRequired();
        builder.HasIndex(doctor => doctor.LicenseNumber).IsUnique();
    }
}

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("appointments");
        builder.HasKey(appointment => appointment.Id);
        builder.Property(appointment => appointment.Reason).HasMaxLength(200).IsRequired();
        builder.Property(appointment => appointment.Notes).HasMaxLength(1000);
        builder.Property(appointment => appointment.Status).HasConversion<string>().HasMaxLength(16);
        builder.Property(appointment => appointment.ScheduledAtUtc).IsRequired();

        builder.HasOne(appointment => appointment.Patient)
            .WithMany(patient => patient.Appointments)
            .HasForeignKey(appointment => appointment.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(appointment => appointment.Doctor)
            .WithMany(doctor => doctor.Appointments)
            .HasForeignKey(appointment => appointment.DoctorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(appointment => new { appointment.DoctorId, appointment.ScheduledAtUtc });
    }
}
