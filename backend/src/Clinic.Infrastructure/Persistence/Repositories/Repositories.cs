using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Persistence.Repositories;

public sealed class PatientRepository(ClinicDbContext db) : IPatientRepository
{
    public Task<PagedResult<Patient>> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = ApplySearch(db.Patients.AsNoTracking(), search)
            .OrderBy(patient => patient.LastName)
            .ThenBy(patient => patient.FirstName);
        return QueryPages.ReadAsync(query, page, pageSize, cancellationToken);
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
    public Task<PagedResult<Doctor>> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Doctors.AsNoTracking().Include(doctor => doctor.Specialty).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search.Trim()}%";
            query = query.Where(doctor =>
                EF.Functions.ILike(doctor.FirstName, pattern) ||
                EF.Functions.ILike(doctor.LastName, pattern) ||
                EF.Functions.ILike(doctor.Specialty.Name, pattern) ||
                EF.Functions.ILike(doctor.LicenseNumber, pattern) ||
                EF.Functions.ILike(doctor.Phone, pattern));
        }

        return QueryPages.ReadAsync(
            query.OrderBy(doctor => doctor.LastName).ThenBy(doctor => doctor.FirstName),
            page,
            pageSize,
            cancellationToken);
    }

    public Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Doctors.Include(doctor => doctor.Specialty).FirstOrDefaultAsync(doctor => doctor.Id == id, cancellationToken);

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
    public Task<PagedResult<Appointment>> ListAsync(string? search, int page, int pageSize, CancellationToken cancellationToken)
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
                EF.Functions.ILike(appointment.Doctor.Specialty.Name, pattern));
        }

        return QueryPages.ReadAsync(query.OrderBy(appointment => appointment.ScheduledAtUtc), page, pageSize, cancellationToken);
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
            .ThenInclude(doctor => doctor.Specialty)
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
        db.Appointments.AsNoTracking()
            .Include(appointment => appointment.Patient)
            .Include(appointment => appointment.Doctor)
            .ThenInclude(doctor => doctor.Specialty);
}

public sealed class SpecialtyRepository(ClinicDbContext db) : ISpecialtyRepository
{
    public async Task<IReadOnlyList<Specialty>> ListAsync(CancellationToken cancellationToken) =>
        await db.Specialties.AsNoTracking().OrderBy(specialty => specialty.Name).ToListAsync(cancellationToken);

    public Task<Specialty?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        db.Specialties.FirstOrDefaultAsync(specialty => specialty.Id == id, cancellationToken);

    public Task<Specialty?> FindByNameAsync(string name, CancellationToken cancellationToken) =>
        db.Specialties.FirstOrDefaultAsync(specialty => EF.Functions.ILike(specialty.Name, name), cancellationToken);

    public Task AddAsync(Specialty specialty, CancellationToken cancellationToken) =>
        db.Specialties.AddAsync(specialty, cancellationToken).AsTask();
}

file static class QueryPages
{
    public static async Task<PagedResult<T>> ReadAsync<T>(
        IOrderedQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (current, size) = PageQuery.Normalize(page, pageSize);
        var total = await query.CountAsync(cancellationToken);
        var pages = size <= 0 ? 0 : (int)Math.Ceiling(total / (double)size);
        var resolved = pages == 0 ? 1 : Math.Min(current, pages);
        var items = total == 0
            ? new List<T>()
            : await query.Skip((resolved - 1) * size).Take(size).ToListAsync(cancellationToken);

        return new PagedResult<T>(items, resolved, size, total);
    }
}
