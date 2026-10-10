using Clinic.Application.Interfaces;
using Clinic.Application.Common;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Clinic.Infrastructure.Persistence;

public sealed class UnitOfWork(ClinicDbContext db) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException postgres)
        {
            if (postgres.SqlState == PostgresErrorCodes.UniqueViolation)
                throw new AppValidationException(["این مقدار یکتا قبلاً ثبت شده است."]);

            if (postgres.SqlState == PostgresErrorCodes.ForeignKeyViolation)
                throw new AppValidationException(["این مورد به نوبت وصل است و نمی‌توان آن را حذف کرد."]);

            throw;
        }
    }
}
