# کلینیک من

فرانت‌اند Angular و بک‌اند ASP.NET Core 10 از هم جدا هستند. بک‌اند با معماری کلین و Entity Framework Core به PostgreSQL وصل می‌شود.

## لایه‌ها

- `backend/src/Clinic.Domain`: موجودیت‌ها و قوانین دامنه، بدون وابستگی بیرونی
- `backend/src/Clinic.Application`: سرویس‌ها در `services`، اینترفیس‌ها در `interfaces` و مدل‌های انتقال داده در `Dtos`
- `backend/src/Clinic.Infrastructure`: EF Core، ریپازیتوری‌ها و PostgreSQL
- `backend/src/Clinic.Api`: کنترلرها و نقطه ورود
- `frontend`: رابط پذیرش

## اجرا

PostgreSQL را با Docker بالا بیاورید:

```powershell
docker compose up -d
```

API روی `http://localhost:5080` و Swagger روی `http://localhost:5080/swagger`:

```powershell
cd backend
dotnet run --project src/Clinic.Api
```

رابط کاربری روی `http://localhost:4200`:

```powershell
cd frontend
npm start
```

رشته اتصال در `backend/src/Clinic.Api/appsettings.Development.json` است. مایگریشن را خودتان اعمال می‌کنید:

```powershell
cd backend
dotnet ef database update --project src/Clinic.Infrastructure --startup-project src/Clinic.Api
```
