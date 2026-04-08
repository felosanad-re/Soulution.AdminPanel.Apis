using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPriceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalReportTransactionPrice",
                table: "ReportTransactions",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "ReportTransactionItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // To update TotalPrice value in ReportTransactionItem
            migrationBuilder.Sql(@"
                UPDATE RTI 
                SET TotalPrice = Price * Quantity
                FROM ReportTransactionItem RTI
            ");

            // To update TotalReportTransactionPrice in ReportTransactions
            migrationBuilder.Sql(@"
                UPDATE RT
                SET TotalReportTransactionPrice = ISNULL((
                    SELECT SUM(RTI.TotalPrice)
                    FROM ReportTransactionItem RTI
                    WHERE RTI.ReportTransactionId = RT.Id
                ),0)
                FROM ReportTransactions RT;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalReportTransactionPrice",
                table: "ReportTransactions");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "ReportTransactionItem");
        }
    }
}
