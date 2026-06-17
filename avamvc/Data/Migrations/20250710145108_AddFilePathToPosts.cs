using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace avamvc.Data.Migrations {
	/// <inheritdoc />
	public partial class AddFilePathToPosts : Migration {
		/// <inheritdoc />
		protected override void Up(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropForeignKey(
				name: "FK_Posts_AspNetUsers_AuthorId",
				table: "Posts");

			migrationBuilder.AlterColumn<string>(
				name: "FilePath",
				table: "Posts",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text");

			migrationBuilder.AlterColumn<string>(
				name: "AuthorId",
				table: "Posts",
				type: "text",
				nullable: true,
				oldClrType: typeof(string),
				oldType: "text");

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_AspNetUsers_AuthorId",
				table: "Posts",
				column: "AuthorId",
				principalTable: "AspNetUsers",
				principalColumn: "Id");
		}

		/// <inheritdoc />
		protected override void Down(MigrationBuilder migrationBuilder) {
			migrationBuilder.DropForeignKey(
				name: "FK_Posts_AspNetUsers_AuthorId",
				table: "Posts");

			migrationBuilder.AlterColumn<string>(
				name: "FilePath",
				table: "Posts",
				type: "text",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AlterColumn<string>(
				name: "AuthorId",
				table: "Posts",
				type: "text",
				nullable: false,
				defaultValue: "",
				oldClrType: typeof(string),
				oldType: "text",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_Posts_AspNetUsers_AuthorId",
				table: "Posts",
				column: "AuthorId",
				principalTable: "AspNetUsers",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
