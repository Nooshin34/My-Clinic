using System.Globalization;
using Clinic.Application.Common;
using Clinic.Application.Dtos;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services;

public sealed class AvailabilityService(
    IAvailabilityRepository slots,
    IDoctorRepository doctors,
    IAppointmentRepository appointments,
    IUnitOfWork unitOfWork) : IAvailabilityService
{
    private static readonly int[] AllowedDurations = [15, 20, 30, 45, 60];

    public async Task<IReadOnlyList<AvailabilitySlotDto>> ListAsync(
        Guid doctorId,
        string? from,
        string? to,
        CancellationToken cancellationToken)
    {
        _ = await doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("پزشک پیدا نشد.");

        var start = ParseDateOrNull(from) ?? ClinicTime.Today();
        var end = ParseDateOrNull(to) ?? start.AddDays(60);
        if (end < start)
            (start, end) = (end, start);
        if (end.DayNumber - start.DayNumber > 120)
            end = start.AddDays(120);

        return await MapAsync(doctorId, start, end, cancellationToken);
    }

    public async Task<IReadOnlyList<AvailabilitySlotDto>> CreateAsync(
        Guid doctorId,
        CreateAvailabilityRequest request,
        CancellationToken cancellationToken)
    {
        _ = await doctors.GetByIdAsync(doctorId, cancellationToken)
            ?? throw new NotFoundException("پزشک پیدا نشد.");

        var errors = new List<string>();
        if (!TryParseDate(request.Date, out var date))
            errors.Add("تاریخ معتبر نیست.");
        if (!TryParseTime(request.StartTime, out var startTime))
            errors.Add("ساعت شروع معتبر نیست.");

        TimeOnly? endTime = null;
        if (!string.IsNullOrWhiteSpace(request.EndTime))
        {
            if (!TryParseTime(request.EndTime, out var parsedEnd))
                errors.Add("ساعت پایان معتبر نیست.");
            else
                endTime = parsedEnd;
        }

        if (!AllowedDurations.Contains(request.DurationMinutes))
            errors.Add("مدت هر نوبت باید ۱۵، ۲۰، ۳۰، ۴۵ یا ۶۰ دقیقه باشد.");

        var visitKind = ClinicEnums.ParseVisitKind(request.VisitKind);
        if (visitKind is null)
            errors.Add("نوع ویزیت را حضوری یا آنلاین انتخاب کنید.");

        if (errors.Count > 0)
            throw new AppValidationException(errors);

        if (date < ClinicTime.Today())
            throw new AppValidationException(["برای تاریخ گذشته نمی‌شود ساعت ثبت کرد."]);

        if (date > ClinicTime.Today().AddDays(180))
            throw new AppValidationException(["برنامه را حداکثر تا شش ماه آینده می‌شود ثبت کرد."]);

        var starts = BuildStarts(startTime, endTime, request.DurationMinutes);
        var existing = await slots.ListAsync(doctorId, date, date, cancellationToken);
        var fresh = new List<TimeOnly>();

        foreach (var start in starts)
        {
            var same = existing.FirstOrDefault(slot => slot.StartTime == start);
            if (same is not null)
            {
                if (same.DurationMinutes != request.DurationMinutes || same.VisitKind != visitKind)
                    throw new AppValidationException([$"ساعت {start:HH:mm} قبلاً با تنظیم دیگری ثبت شده است."]);
                continue;
            }

            if (existing.Any(slot => Overlaps(start, request.DurationMinutes, slot.StartTime, slot.DurationMinutes)) ||
                fresh.Any(other => Overlaps(start, request.DurationMinutes, other, request.DurationMinutes)))
            {
                throw new AppValidationException(["این بازه با ساعت‌های قبلی تداخل دارد."]);
            }

            fresh.Add(start);
        }

        if (fresh.Count == 0)
            throw new AppValidationException(["این ساعت‌ها قبلاً در برنامه ثبت شده‌اند."]);

        await slots.AddRangeAsync(fresh.Select(start => new AvailabilitySlot
        {
            DoctorId = doctorId,
            Date = date,
            StartTime = start,
            DurationMinutes = request.DurationMinutes,
            VisitKind = visitKind!.Value
        }), cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await MapAsync(doctorId, date, date, cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var slot = await slots.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException("این ساعت پیدا نشد.");

        var startsAt = ClinicTime.ToUtc(slot.Date, slot.StartTime);
        if (await appointments.ExistsForDoctorAtAsync(slot.DoctorId, startsAt, null, cancellationToken))
            throw new AppValidationException(["این ساعت رزرو شده و حذف نمی‌شود."]);

        slots.Remove(slot);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<AvailabilitySlotDto>> MapAsync(
        Guid doctorId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken)
    {
        var items = await slots.ListAsync(doctorId, from, to, cancellationToken);
        var busy = (await appointments.BusyTimesAsync(
                doctorId,
                ClinicTime.ToUtc(from, TimeOnly.MinValue),
                ClinicTime.ToUtc(to.AddDays(1), TimeOnly.MinValue),
                cancellationToken))
            .Select(ClinicTime.Minute)
            .ToHashSet();

        return items.Select(slot =>
        {
            var startsAt = ClinicTime.ToUtc(slot.Date, slot.StartTime);
            return new AvailabilitySlotDto(
                slot.Id,
                slot.DoctorId,
                slot.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                slot.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                slot.DurationMinutes,
                slot.VisitKind.ToString(),
                startsAt,
                busy.Contains(startsAt));
        }).ToList();
    }

    private static List<TimeOnly> BuildStarts(TimeOnly start, TimeOnly? end, int durationMinutes)
    {
        var startMinute = Minutes(start);
        if (startMinute + durationMinutes > DayMinutes)
            throw new AppValidationException(["این ساعت از پایان روز رد می‌شود."]);

        if (end is null)
            return [start];

        var endMinute = Minutes(end.Value);
        if (endMinute <= startMinute)
            throw new AppValidationException(["ساعت پایان باید بعد از ساعت شروع باشد."]);

        var result = new List<TimeOnly>();
        for (var cursor = startMinute; cursor + durationMinutes <= endMinute; cursor += durationMinutes)
        {
            result.Add(TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(cursor)));
            if (result.Count > 48)
                throw new AppValidationException(["در یک روز بیشتر از ۴۸ ساعت نمی‌شود ثبت کرد."]);
        }

        if (result.Count == 0)
            throw new AppValidationException(["بازه زمانی برای این مدت کوتاه است."]);

        return result;
    }

    private const int DayMinutes = 24 * 60;

    private static int Minutes(TimeOnly time) => (int)time.ToTimeSpan().TotalMinutes;

    private static bool Overlaps(TimeOnly left, int leftMinutes, TimeOnly right, int rightMinutes)
    {
        var leftStart = left.ToTimeSpan().TotalMinutes;
        var rightStart = right.ToTimeSpan().TotalMinutes;
        return leftStart < rightStart + rightMinutes && rightStart < leftStart + leftMinutes;
    }

    private static DateOnly? ParseDateOrNull(string? value) =>
        TryParseDate(value, out var date) ? date : null;

    private static bool TryParseDate(string? value, out DateOnly date) =>
        DateOnly.TryParseExact(value?.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

    private static bool TryParseTime(string? value, out TimeOnly time) =>
        TimeOnly.TryParseExact(value?.Trim(), ["HH:mm", "H:mm"], CultureInfo.InvariantCulture, DateTimeStyles.None, out time);
}
