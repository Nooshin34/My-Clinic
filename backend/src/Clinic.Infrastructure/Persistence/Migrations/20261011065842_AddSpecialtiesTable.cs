using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialtiesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "specialties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_specialties", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "specialties",
                columns: new[] { "Id", "CreatedAtUtc", "Name" },
                values: new object[,]
                {
                    { new Guid("8f0a6c10-0001-4000-8000-000000000001"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "پزشکی عمومی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000002"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "قلب و عروق" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000003"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "کودکان" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000004"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "ارتوپدی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000005"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "پوست" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000006"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "چشم" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000007"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "گوش و حلق و بینی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000008"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "زنان و زایمان" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000009"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "مغز و اعصاب" },
                    { new Guid("8f0a6c10-0001-4000-8000-00000000000a"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "دندان‌پزشکی" },
                    { new Guid("8f0a6c10-0001-4000-8000-00000000000b"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "اورژانس" },
                    { new Guid("8f0a6c10-0001-4000-8000-00000000000c"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "روان‌پزشکی" },
                    { new Guid("8f0a6c10-0001-4000-8000-00000000000d"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "ریه" },
                    { new Guid("8f0a6c10-0001-4000-8000-00000000000e"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "گوارش" },
                    { new Guid("8f0a6c10-0001-4000-8000-00000000000f"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "کلیه و مجاری ادرار" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000010"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "غدد" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000011"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "عفونی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000012"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "رادیولوژی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000013"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "بیهوشی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000014"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "جراحی" },
                    { new Guid("8f0a6c10-0001-4000-8000-000000000015"), new DateTime(2026, 10, 11, 0, 0, 0, 0, DateTimeKind.Utc), "داخلی" }
                });

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "doctors",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE doctors AS d
                SET "SpecialtyId" = CASE btrim(d."Specialty")
                    WHEN 'General' THEN '8f0a6c10-0001-4000-8000-000000000001'::uuid
                    WHEN 'پزشکی عمومی' THEN '8f0a6c10-0001-4000-8000-000000000001'::uuid
                    WHEN 'Cardiology' THEN '8f0a6c10-0001-4000-8000-000000000002'::uuid
                    WHEN 'قلب و عروق' THEN '8f0a6c10-0001-4000-8000-000000000002'::uuid
                    WHEN 'Pediatrics' THEN '8f0a6c10-0001-4000-8000-000000000003'::uuid
                    WHEN 'کودکان' THEN '8f0a6c10-0001-4000-8000-000000000003'::uuid
                    WHEN 'Orthopedics' THEN '8f0a6c10-0001-4000-8000-000000000004'::uuid
                    WHEN 'ارتوپدی' THEN '8f0a6c10-0001-4000-8000-000000000004'::uuid
                    WHEN 'Dermatology' THEN '8f0a6c10-0001-4000-8000-000000000005'::uuid
                    WHEN 'پوست' THEN '8f0a6c10-0001-4000-8000-000000000005'::uuid
                    WHEN 'Ophthalmology' THEN '8f0a6c10-0001-4000-8000-000000000006'::uuid
                    WHEN 'چشم' THEN '8f0a6c10-0001-4000-8000-000000000006'::uuid
                    WHEN 'Otolaryngology' THEN '8f0a6c10-0001-4000-8000-000000000007'::uuid
                    WHEN 'گوش و حلق و بینی' THEN '8f0a6c10-0001-4000-8000-000000000007'::uuid
                    WHEN 'Obstetrics' THEN '8f0a6c10-0001-4000-8000-000000000008'::uuid
                    WHEN 'زنان و زایمان' THEN '8f0a6c10-0001-4000-8000-000000000008'::uuid
                    WHEN 'Neurology' THEN '8f0a6c10-0001-4000-8000-000000000009'::uuid
                    WHEN 'مغز و اعصاب' THEN '8f0a6c10-0001-4000-8000-000000000009'::uuid
                    WHEN 'Dentistry' THEN '8f0a6c10-0001-4000-8000-00000000000a'::uuid
                    WHEN 'دندان‌پزشکی' THEN '8f0a6c10-0001-4000-8000-00000000000a'::uuid
                    WHEN 'Emergency' THEN '8f0a6c10-0001-4000-8000-00000000000b'::uuid
                    WHEN 'اورژانس' THEN '8f0a6c10-0001-4000-8000-00000000000b'::uuid
                    WHEN 'Psychiatry' THEN '8f0a6c10-0001-4000-8000-00000000000c'::uuid
                    WHEN 'روان‌پزشکی' THEN '8f0a6c10-0001-4000-8000-00000000000c'::uuid
                    WHEN 'Pulmonology' THEN '8f0a6c10-0001-4000-8000-00000000000d'::uuid
                    WHEN 'ریه' THEN '8f0a6c10-0001-4000-8000-00000000000d'::uuid
                    WHEN 'Gastroenterology' THEN '8f0a6c10-0001-4000-8000-00000000000e'::uuid
                    WHEN 'گوارش' THEN '8f0a6c10-0001-4000-8000-00000000000e'::uuid
                    WHEN 'Urology' THEN '8f0a6c10-0001-4000-8000-00000000000f'::uuid
                    WHEN 'کلیه و مجاری ادرار' THEN '8f0a6c10-0001-4000-8000-00000000000f'::uuid
                    WHEN 'Endocrinology' THEN '8f0a6c10-0001-4000-8000-000000000010'::uuid
                    WHEN 'غدد' THEN '8f0a6c10-0001-4000-8000-000000000010'::uuid
                    WHEN 'InfectiousDisease' THEN '8f0a6c10-0001-4000-8000-000000000011'::uuid
                    WHEN 'عفونی' THEN '8f0a6c10-0001-4000-8000-000000000011'::uuid
                    WHEN 'Radiology' THEN '8f0a6c10-0001-4000-8000-000000000012'::uuid
                    WHEN 'رادیولوژی' THEN '8f0a6c10-0001-4000-8000-000000000012'::uuid
                    WHEN 'Anesthesiology' THEN '8f0a6c10-0001-4000-8000-000000000013'::uuid
                    WHEN 'بیهوشی' THEN '8f0a6c10-0001-4000-8000-000000000013'::uuid
                    WHEN 'Surgery' THEN '8f0a6c10-0001-4000-8000-000000000014'::uuid
                    WHEN 'جراحی' THEN '8f0a6c10-0001-4000-8000-000000000014'::uuid
                    WHEN 'InternalMedicine' THEN '8f0a6c10-0001-4000-8000-000000000015'::uuid
                    WHEN 'داخلی' THEN '8f0a6c10-0001-4000-8000-000000000015'::uuid
                    ELSE NULL
                END;

                INSERT INTO specialties ("Id", "Name", "CreatedAtUtc")
                SELECT gen_random_uuid(), btrim(d."Specialty"), TIMESTAMPTZ '2026-10-11 00:00:00+00'
                FROM doctors AS d
                WHERE d."SpecialtyId" IS NULL AND btrim(d."Specialty") <> ''
                GROUP BY btrim(d."Specialty");

                UPDATE doctors AS d
                SET "SpecialtyId" = s."Id"
                FROM specialties AS s
                WHERE d."SpecialtyId" IS NULL AND btrim(d."Specialty") = s."Name";

                UPDATE doctors
                SET "SpecialtyId" = '8f0a6c10-0001-4000-8000-000000000001'::uuid
                WHERE "SpecialtyId" IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "SpecialtyId",
                table: "doctors",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_doctors_SpecialtyId",
                table: "doctors",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_specialties_Name",
                table: "specialties",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_doctors_specialties_SpecialtyId",
                table: "doctors",
                column: "SpecialtyId",
                principalTable: "specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "Specialty",
                table: "doctors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Specialty",
                table: "doctors",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE doctors AS d
                SET "Specialty" = s."Name"
                FROM specialties AS s
                WHERE d."SpecialtyId" = s."Id";

                UPDATE doctors
                SET "Specialty" = 'پزشکی عمومی'
                WHERE "Specialty" IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "doctors",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80,
                oldNullable: true);

            migrationBuilder.DropForeignKey(
                name: "FK_doctors_specialties_SpecialtyId",
                table: "doctors");

            migrationBuilder.DropIndex(
                name: "IX_doctors_SpecialtyId",
                table: "doctors");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "doctors");

            migrationBuilder.DropTable(
                name: "specialties");
        }
    }
}
