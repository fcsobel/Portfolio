using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Portfolio.Data.Context.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    StockId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ticker = table.Column<string>(maxLength: 10, nullable: true),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(19, 4)", nullable: false),
                    Dividend = table.Column<decimal>(type: "decimal(19, 5)", nullable: false),
                    PriceTargetUpdated = table.Column<DateTime>(nullable: false),
                    PriceTargetAverage = table.Column<decimal>(type: "decimal(19, 5)", nullable: false),
                    PriceTargetHigh = table.Column<decimal>(type: "decimal(19, 5)", nullable: false),
                    PriceTargetLow = table.Column<decimal>(type: "decimal(19, 5)", nullable: false),
                    PriceTargetAnalysts = table.Column<int>(nullable: false),
                    DividendGrowthRate5Y = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    DividendPerShare5Y = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    DividendPerShareAnnual = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    DividendYield5Y = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    DividendYieldIndicatedAnnual = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    DividendsPerShareTTM = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    High_52Week = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    HighDate_52Week = table.Column<DateTime>(nullable: true),
                    Low_52Week = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    LowDate_52Week = table.Column<DateTime>(nullable: true),
                    AverageTradingVolume10Day = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    PriceReturnDaily13Week = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    PriceReturnDaily26Week = table.Column<decimal>(type: "decimal(19, 5)", nullable: true),
                    CacheDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.StockId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: true),
                    UserName = table.Column<string>(maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(maxLength: 256, nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "StockDividends",
                columns: table => new
                {
                    StockId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Dividend = table.Column<decimal>(type: "decimal(19, 4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockDividends", x => new { x.StockId, x.Date });
                    table.ForeignKey(
                        name: "FK_StockDividends_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockPrice",
                columns: table => new
                {
                    StockId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(19, 4)", nullable: false),
                    Close = table.Column<decimal>(type: "decimal(19, 4)", nullable: false),
                    Low = table.Column<decimal>(type: "decimal(19, 4)", nullable: false),
                    High = table.Column<decimal>(type: "decimal(19, 4)", nullable: false),
                    Volume = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockPrice", x => new { x.StockId, x.Date });
                    table.ForeignKey(
                        name: "FK_StockPrice_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Url = table.Column<string>(maxLength: 100, nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    OwnerUserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    ProviderName = table.Column<string>(maxLength: 10, nullable: false),
                    ProviderKey = table.Column<string>(nullable: true),
                    UserName = table.Column<string>(maxLength: 50, nullable: true),
                    LoginCount = table.Column<int>(nullable: false),
                    LoginDate = table.Column<DateTime>(nullable: true),
                    Url = table.Column<string>(maxLength: 1000, nullable: true),
                    Picture = table.Column<string>(maxLength: 1000, nullable: true),
                    Created = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginProfiles", x => new { x.UserId, x.ProviderName });
                    table.ForeignKey(
                        name: "FK_LoginProfiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Investments",
                columns: table => new
                {
                    InvestmentId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<long>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(19, 3)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(19, 2)", nullable: false),
                    StockId = table.Column<int>(nullable: false),
                    ReInvest = table.Column<bool>(nullable: false),
                    PurchaseDate = table.Column<DateTime>(nullable: false),
                    InvestmentType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investments", x => x.InvestmentId);
                    table.ForeignKey(
                        name: "FK_Investments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Investments_Stocks_StockId",
                        column: x => x.StockId,
                        principalTable: "Stocks",
                        principalColumn: "StockId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_OwnerUserId",
                table: "Accounts",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_UserId",
                table: "Accounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_AccountId",
                table: "Investments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Investments_StockId",
                table: "Investments",
                column: "StockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investments");

            migrationBuilder.DropTable(
                name: "LoginProfiles");

            migrationBuilder.DropTable(
                name: "StockDividends");

            migrationBuilder.DropTable(
                name: "StockPrice");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
