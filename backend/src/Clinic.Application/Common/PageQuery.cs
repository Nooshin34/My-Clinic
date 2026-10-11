namespace Clinic.Application.Common;

public static class PageQuery
{
    public const int DefaultSize = 10;
    public const int MaxSize = 100;

    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        var normalizedPage = page < 1 ? 1 : page;
        var normalizedSize = pageSize < 1 ? DefaultSize : Math.Min(pageSize, MaxSize);
        return (normalizedPage, normalizedSize);
    }
}
