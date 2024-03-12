using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proto.Module.Migrations
{
    /// <inheritdoc />
    public partial class TlabsBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Identity");

            migrationBuilder.CreateTable(
                name: "ApiKey",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TokenName = table.Column<string>(type: "TEXT", nullable: true),
                    Hash = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ValidityState = table.Column<string>(type: "TEXT", nullable: false),
                    Editor = table.Column<string>(type: "TEXT", nullable: true),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKey", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditRecord",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Method = table.Column<string>(type: "TEXT", nullable: true),
                    ActionName = table.Column<string>(type: "TEXT", nullable: true),
                    URL = table.Column<string>(type: "TEXT", nullable: true),
                    BodyData = table.Column<string>(type: "TEXT", nullable: true),
                    Error = table.Column<string>(type: "TEXT", nullable: true),
                    IPAddress = table.Column<string>(type: "TEXT", nullable: true),
                    StatusCode = table.Column<string>(type: "TEXT", nullable: true),
                    Editor = table.Column<string>(type: "TEXT", nullable: true),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locale",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Lang = table.Column<string>(type: "TEXT", nullable: true),
                    DecimalSep = table.Column<string>(type: "TEXT", nullable: true),
                    ThousandSep = table.Column<string>(type: "TEXT", nullable: true),
                    ListSep = table.Column<string>(type: "TEXT", nullable: true),
                    DateFormat = table.Column<string>(type: "TEXT", nullable: true),
                    TimeFormat = table.Column<string>(type: "TEXT", nullable: true),
                    DateTimeFormat = table.Column<string>(type: "TEXT", nullable: true),
                    IntegerFormat = table.Column<string>(type: "TEXT", nullable: true),
                    FixedFormat = table.Column<string>(type: "TEXT", nullable: true),
                    MonetaryFormat = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locale", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedRoleName = table.Column<string>(type: "TEXT", nullable: true),
                    AllowAccessPattern = table.Column<string>(type: "TEXT", nullable: true),
                    DenyAccessPattern = table.Column<string>(type: "TEXT", nullable: true),
                    EnforcedFilters = table.Column<string>(type: "TEXT", nullable: true),
                    Editor = table.Column<string>(type: "TEXT", nullable: true),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "Identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Firstname = table.Column<string>(type: "TEXT", nullable: true),
                    Lastname = table.Column<string>(type: "TEXT", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false),
                    LocaleId = table.Column<int>(type: "INTEGER", nullable: true),
                    Editor = table.Column<string>(type: "TEXT", nullable: true),
                    Modified = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Locale_LocaleId",
                        column: x => x.LocaleId,
                        principalSchema: "Identity",
                        principalTable: "Locale",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ApiKeyRoleRef",
                schema: "Identity",
                columns: table => new
                {
                    ApiKeyId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeyRoleRef", x => new { x.ApiKeyId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ApiKeyRoleRef_ApiKey_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalSchema: "Identity",
                        principalTable: "ApiKey",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiKeyRoleRef_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleRef",
                schema: "Identity",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleRef", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoleRef_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Identity",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleRef_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "Identity",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKey_Hash",
                schema: "Identity",
                table: "ApiKey",
                column: "Hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKey_TokenName",
                schema: "Identity",
                table: "ApiKey",
                column: "TokenName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeyRoleRef_RoleId",
                schema: "Identity",
                table: "ApiKeyRoleRef",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditRecord_Id",
                schema: "Identity",
                table: "AuditRecord",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Locale_Lang",
                schema: "Identity",
                table: "Locale",
                column: "Lang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                schema: "Identity",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                schema: "Identity",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_LocaleId",
                schema: "Identity",
                table: "User",
                column: "LocaleId");

            migrationBuilder.CreateIndex(
                name: "IX_User_UserName",
                schema: "Identity",
                table: "User",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleRef_RoleId",
                schema: "Identity",
                table: "UserRoleRef",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiKeyRoleRef",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "AuditRecord",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "UserRoleRef",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "ApiKey",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "User",
                schema: "Identity");

            migrationBuilder.DropTable(
                name: "Locale",
                schema: "Identity");
        }
    }
}
