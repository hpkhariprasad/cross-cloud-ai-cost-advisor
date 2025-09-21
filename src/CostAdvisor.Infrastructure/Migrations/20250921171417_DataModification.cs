using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CostAdvisor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DataModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:hstore", ",,");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Recommendations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedSavings",
                table: "Recommendations",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "BillingAccount",
                table: "NormalizedCosts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResourceName",
                table: "NormalizedCosts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Dictionary<string, string>>(
                name: "Tags",
                table: "NormalizedCosts",
                type: "hstore",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "EstimatedSavings",
                table: "Recommendations");

            migrationBuilder.DropColumn(
                name: "BillingAccount",
                table: "NormalizedCosts");

            migrationBuilder.DropColumn(
                name: "ResourceName",
                table: "NormalizedCosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "NormalizedCosts");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:hstore", ",,");
        }
    }
}
