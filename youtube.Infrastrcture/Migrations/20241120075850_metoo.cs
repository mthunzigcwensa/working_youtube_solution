﻿using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace youtube.Infrastrcture.Migrations
{
    /// <inheritdoc />
    public partial class metoo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AddByDate",
                table: "Videos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddByDate",
                table: "Videos");
        }
    }
}
