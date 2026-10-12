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
        builder.Property(doctor => doctor.Phone).HasMaxLength(20).IsRequired();
        builder.Property(doctor => doctor.Email).HasMaxLength(120).IsRequired();
        builder.HasIndex(doctor => doctor.Email).IsUnique();
        builder.Property(doctor => doctor.LicenseNumber).HasMaxLength(32).IsRequired();
        builder.HasIndex(doctor => doctor.LicenseNumber).IsUnique();

        builder.HasOne(doctor => doctor.Specialty)
            .WithMany(specialty => specialty.Doctors)
            .HasForeignKey(doctor => doctor.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public static readonly DateTime SeededAtUtc = new(2026, 10, 11, 0, 0, 0, DateTimeKind.Utc);

    public static readonly (Guid Id, string Name)[] Seeds =
    [
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000001"), "پزشکی عمومی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000002"), "قلب و عروق"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000003"), "کودکان"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000004"), "ارتوپدی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000005"), "پوست"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000006"), "چشم"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000007"), "گوش و حلق و بینی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000008"), "زنان و زایمان"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000009"), "مغز و اعصاب"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-00000000000a"), "دندان‌پزشکی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-00000000000b"), "اورژانس"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-00000000000c"), "روان‌پزشکی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-00000000000d"), "ریه"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-00000000000e"), "گوارش"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-00000000000f"), "کلیه و مجاری ادرار"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000010"), "غدد"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000011"), "عفونی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000012"), "رادیولوژی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000013"), "بیهوشی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000014"), "جراحی"),
        (Guid.Parse("8f0a6c10-0001-4000-8000-000000000015"), "داخلی")
    ];

    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        builder.ToTable("specialties");
        builder.HasKey(specialty => specialty.Id);
        builder.Property(specialty => specialty.Name).HasMaxLength(80).IsRequired();
        builder.HasIndex(specialty => specialty.Name).IsUnique();
        builder.HasData(Seeds.Select(item => new Specialty
        {
            Id = item.Id,
            Name = item.Name,
            CreatedAtUtc = SeededAtUtc
        }));
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

        builder.Property(appointment => appointment.VisitKind).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(appointment => new { appointment.DoctorId, appointment.ScheduledAtUtc });
    }
}

public class AvailabilitySlotConfiguration : IEntityTypeConfiguration<AvailabilitySlot>
{
    public void Configure(EntityTypeBuilder<AvailabilitySlot> builder)
    {
        builder.ToTable("availability_slots");
        builder.HasKey(slot => slot.Id);
        builder.Property(slot => slot.Date).IsRequired();
        builder.Property(slot => slot.StartTime).IsRequired();
        builder.Property(slot => slot.DurationMinutes).IsRequired();
        builder.Property(slot => slot.VisitKind).HasConversion<string>().HasMaxLength(16);

        builder.HasOne(slot => slot.Doctor)
            .WithMany(doctor => doctor.Availability)
            .HasForeignKey(slot => slot.DoctorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(slot => new { slot.DoctorId, slot.Date, slot.StartTime }).IsUnique();
    }
}
