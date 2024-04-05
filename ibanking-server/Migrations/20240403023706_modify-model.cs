using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ibanking_server.Migrations
{
    /// <inheritdoc />
    public partial class modifymodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OTPs_TransactionId",
                table: "OTPs");

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_TransactionId",
                table: "OTPs",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OTPs_TransactionId",
                table: "OTPs");

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_TransactionId",
                table: "OTPs",
                column: "TransactionId",
                unique: true);
        }
    }
}
