using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertSpecialtyToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE doctors SET "Specialty" = CASE
                    WHEN "Specialty" IN (
                        'General', 'Cardiology', 'Pediatrics', 'Orthopedics', 'Dermatology',
                        'Ophthalmology', 'Otolaryngology', 'Obstetrics', 'Neurology', 'Dentistry',
                        'Emergency', 'Psychiatry', 'Pulmonology', 'Gastroenterology', 'Urology',
                        'Endocrinology', 'InfectiousDisease', 'Radiology', 'Anesthesiology',
                        'Surgery', 'InternalMedicine') THEN "Specialty"
                    WHEN "Specialty" ILIKE '%قلب%' OR "Specialty" ILIKE '%عروق%' OR "Specialty" ILIKE '%کاردیو%' THEN 'Cardiology'
                    WHEN "Specialty" ILIKE '%کودک%' OR "Specialty" ILIKE '%اطفال%' OR "Specialty" ILIKE '%نوزاد%' THEN 'Pediatrics'
                    WHEN "Specialty" ILIKE '%ارتوپد%' OR "Specialty" ILIKE '%استخوان%' THEN 'Orthopedics'
                    WHEN "Specialty" ILIKE '%پوست%' THEN 'Dermatology'
                    WHEN "Specialty" ILIKE '%چشم%' THEN 'Ophthalmology'
                    WHEN "Specialty" ILIKE '%گوش%' OR "Specialty" ILIKE '%حلق%' OR "Specialty" ILIKE '%بینی%' THEN 'Otolaryngology'
                    WHEN "Specialty" ILIKE '%زنان%' OR "Specialty" ILIKE '%زایمان%' OR "Specialty" ILIKE '%مامایی%' THEN 'Obstetrics'
                    WHEN "Specialty" ILIKE '%مغز%' OR "Specialty" ILIKE '%اعصاب%' THEN 'Neurology'
                    WHEN "Specialty" ILIKE '%دندان%' THEN 'Dentistry'
                    WHEN "Specialty" ILIKE '%اورژانس%' THEN 'Emergency'
                    WHEN "Specialty" ILIKE '%روان%' THEN 'Psychiatry'
                    WHEN "Specialty" ILIKE '%ریه%' THEN 'Pulmonology'
                    WHEN "Specialty" ILIKE '%گوارش%' THEN 'Gastroenterology'
                    WHEN "Specialty" ILIKE '%کلیه%' OR "Specialty" ILIKE '%ادرار%' THEN 'Urology'
                    WHEN "Specialty" ILIKE '%غدد%' THEN 'Endocrinology'
                    WHEN "Specialty" ILIKE '%عفونی%' THEN 'InfectiousDisease'
                    WHEN "Specialty" ILIKE '%رادیو%' OR "Specialty" ILIKE '%تصویربرداری%' THEN 'Radiology'
                    WHEN "Specialty" ILIKE '%بیهوش%' THEN 'Anesthesiology'
                    WHEN "Specialty" ILIKE '%جراحی%' THEN 'Surgery'
                    WHEN "Specialty" ILIKE '%داخلی%' THEN 'InternalMedicine'
                    ELSE 'General'
                END;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "doctors",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Specialty",
                table: "doctors",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32);
        }
    }
}
