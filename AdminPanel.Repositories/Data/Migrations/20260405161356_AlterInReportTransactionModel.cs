using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterInReportTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "ReportTransactions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "ReportTransactions");
        }
    }
}
