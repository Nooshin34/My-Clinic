namespace Clinic.Application.Common;

public sealed class NotFoundException(string message) : Exception(message);

public sealed class AppValidationException(IReadOnlyList<string> errors)
    : Exception(errors.Count > 0 ? errors[0] : "اطلاعات واردشده معتبر نیست.")
{
    public IReadOnlyList<string> Errors { get; } = errors;
}
