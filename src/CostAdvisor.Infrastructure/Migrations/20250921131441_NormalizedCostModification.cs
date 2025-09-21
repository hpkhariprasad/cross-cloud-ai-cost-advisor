using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CostAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizedCostModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "NormalizedCosts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Cost",
                table: "NormalizedCosts",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
