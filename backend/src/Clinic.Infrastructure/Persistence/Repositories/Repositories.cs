using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository(ClinicDbContext db) : IPatientRepository
{
    public async Task<IReadOnlyList<Patient>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = db.Patients.AsNoTracking().AsQueryable();
        query = ApplySearch(query, search);
        return await query.OrderBy(patient => patient.LastName).ThenBy(patient => patient.FirstName).ToListAsync(cancellationToken);
    }

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Patients.FirstOrDefaultAsync(patient => patient.Id == id, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken) =>
        db.Patients.CountAsync(cancellationToken);

    public Task<bool> NationalIdExistsAsync(string nationalId, Guid? ignoreId, CancellationToken cancellationToken) =>
        db.Patients.AnyAsync(patient => patient.NationalId == nationalId && (ignoreId == null || patient.Id != ignoreId), cancellationToken);

    public Task AddAsync(Patient patient, CancellationToken cancellationToken) =>
        db.Patients.AddAsync(patient, cancellationToken).AsTask();

    public void Remove(Patient patient) => db.Patients.Remove(patient);

    private static IQueryable<Patient> ApplySearch(IQueryable<Patient> query, string? search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return query;

        var pattern = $"%{search.Trim()}%";
        return query.Where(patient =>
            EF.Functions.ILike(patient.FirstName, pattern) ||
            EF.Functions.ILike(patient.LastName, pattern) ||
            EF.Functions.ILike(patient.NationalId, pattern) ||
            EF.Functions.ILike(patient.Phone, pattern));
    }
}

public sealed class DoctorRepository(ClinicDbContext db) : IDoctorRepository
{
    public async Task<IReadOnlyList<Doctor>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = db.Doctors.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(doctor =>
                EF.Functions.ILike(doctor.FirstName, pattern) ||
                EF.Functions.ILike(doctor.LastName, pattern) ||
                EF.Functions.ILike(doctor.Specialty, pattern) ||
                EF.Functions.ILike(doctor.LicenseNumber, pattern) ||
                EF.Functions.ILike(doctor.Phone, pattern));
        }

        return await query.OrderBy(doctor => doctor.LastName).ThenBy(doctor => doctor.FirstName).ToListAsync(cancellationToken);
    }

    public Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Doctors.FirstOrDefaultAsync(doctor => doctor.Id == id, cancellationToken);

    public Task<int> CountAsync(CancellationToken cancellationToken) =>
        db.Doctors.CountAsync(cancellationToken);

    public Task<bool> LicenseExistsAsync(string licenseNumber, Guid? ignoreId, CancellationToken cancellationToken) =>
        db.Doctors.AnyAsync(doctor => doctor.LicenseNumber == licenseNumber && (ignoreId == null || doctor.Id != ignoreId), cancellationToken);

    public Task<bool> EmailExistsAsync(string email, Guid? ignoreId, CancellationToken cancellationToken) =>
        db.Doctors.AnyAsync(doctor => doctor.Email == email && (ignoreId == null || doctor.Id != ignoreId), cancellationToken);

    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken) =>
        db.Doctors.AddAsync(doctor, cancellationToken).AsTask();

    public void Remove(Doctor doctor) => db.Doctors.Remove(doctor);
}

public sealed class AppointmentRepository(ClinicDbContext db) : IAppointmentRepository
{
    public async Task<IReadOnlyList<Appointment>> ListAsync(string? search, CancellationToken cancellationToken)
    {
        var query = DetailedQuery();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(appointment =>
                EF.Functions.ILike(appointment.Reason, pattern) ||
                EF.Functions.ILike(appointment.Patient.FirstName, pattern) ||
                EF.Functions.ILike(appointment.Patient.LastName, pattern) ||
                EF.Functions.ILike(appointment.Doctor.FirstName, pattern) ||
                EF.Functions.ILike(appointment.Doctor.LastName, pattern) ||
                EF.Functions.ILike(appointment.Doctor.Specialty, pattern));
        }

        return await query.OrderBy(appointment => appointment.ScheduledAtUtc).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Appointment>> ListBetweenAsync(DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken) =>
        await DetailedQuery()
            .Where(appointment => appointment.ScheduledAtUtc >= fromUtc && appointment.ScheduledAtUtc < toUtc)
            .OrderBy(appointment => appointment.ScheduledAtUtc)
            .ToListAsync(cancellationToken);

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Appointments
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .FirstOrDefaultAsync(appointment => appointment.Id == id, cancellationToken);

    public Task<int> CountByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken) =>
        db.Appointments.CountAsync(appointment => appointment.Status == status, cancellationToken);

    public Task<bool> AnyForPatientAsync(Guid patientId, CancellationToken cancellationToken) =>
        db.Appointments.AnyAsync(appointment => appointment.PatientId == patientId, cancellationToken);

    public Task<bool> AnyForDoctorAsync(Guid doctorId, CancellationToken cancellationToken) =>
        db.Appointments.AnyAsync(appointment => appointment.DoctorId == doctorId, cancellationToken);

    public Task<bool> ExistsForDoctorAtAsync(Guid doctorId, DateTime scheduledAtUtc, Guid? ignoreId, CancellationToken cancellationToken) =>
        db.Appointments.AnyAsync(appointment =>
            appointment.DoctorId == doctorId &&
            appointment.ScheduledAtUtc == scheduledAtUtc &&
            appointment.Status != AppointmentStatus.Cancelled &&
            (ignoreId == null || appointment.Id != ignoreId), cancellationToken);

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken) =>
        db.Appointments.AddAsync(appointment, cancellationToken).AsTask();

    public void Remove(Appointment appointment) => db.Appointments.Remove(appointment);

    private IQueryable<Appointment> DetailedQuery() =>
        db.Appointments.AsNoTracking().Include(appointment => appointment.Patient).Include(appointment => appointment.Doctor);
}
