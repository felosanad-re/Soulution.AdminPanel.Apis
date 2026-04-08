using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdminPanel.Repositories.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterPurchaseAndSalesTaple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalReportTransactionPrice",
                table: "ReportTransactions",
                newName: "TotalReportTransaction");

            migrationBuilder.RenameColumn(
                name: "TotalPurchase",
                table: "PurchaseInvoices",
                newName: "TotalReportTransaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalReportTransaction",
                table: "ReportTransactions",
                newName: "TotalReportTransactionPrice");

            migrationBuilder.RenameColumn(
                name: "TotalReportTransaction",
                table: "PurchaseInvoices",
                newName: "TotalPurchase");
        }
    }
}
