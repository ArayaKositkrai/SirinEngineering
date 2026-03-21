using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SirinEngineering.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabaseWithPrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_Category",
                columns: table => new
                {
                    C_CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    C_CategoryName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_Category", x => x.C_CategoryID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_Order",
                columns: table => new
                {
                    O_OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    O_OrderDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    O_CustomerName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    O_ProductID = table.Column<int>(type: "int", nullable: false),
                    O_ProductName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    O_Quantity = table.Column<int>(type: "int", nullable: false),
                    O_Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    O_SubTotal = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    O_DiscountAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    O_TotalAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    O_GiftItemName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    O_PaymentType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    O_UserID = table.Column<int>(type: "int", nullable: false),
                    O_Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_Order", x => x.O_OrderID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_Payment",
                columns: table => new
                {
                    PY_PaymentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PY_OrderID = table.Column<int>(type: "int", nullable: false),
                    PY_PaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PY_Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PY_PaymentSlip = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PY_Status = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_Payment", x => x.PY_PaymentID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_Product",
                columns: table => new
                {
                    PD_ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PD_ProductName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PD_ProductImage = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PD_Brand = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PD_CategoryID = table.Column<int>(type: "int", nullable: false),
                    PD_Price = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PD_Cost = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PD_StockQty = table.Column<int>(type: "int", nullable: false),
                    PD_MinStock = table.Column<int>(type: "int", nullable: false),
                    PD_IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_Product", x => x.PD_ProductID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_Promotion",
                columns: table => new
                {
                    PM_PromotionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PM_PromotionName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PM_Detail = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PM_PromoType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PM_MinSpend = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    PM_MinQuantity = table.Column<int>(type: "int", nullable: true),
                    PM_DiscountValue = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    PM_FreeQuantity = table.Column<int>(type: "int", nullable: true),
                    PM_MaxGiftValue = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    PM_TargetCategoryID = table.Column<int>(type: "int", nullable: true),
                    PM_StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PM_EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    PM_IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_Promotion", x => x.PM_PromotionID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_Role",
                columns: table => new
                {
                    R_RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    R_RoleName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_Role", x => x.R_RoleID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TBL_User",
                columns: table => new
                {
                    U_UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    U_Username = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_FullName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_Phone = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U_RoleID = table.Column<int>(type: "int", nullable: false),
                    U_IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TBL_User", x => x.U_UserID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TBL_Category");

            migrationBuilder.DropTable(
                name: "TBL_Order");

            migrationBuilder.DropTable(
                name: "TBL_Payment");

            migrationBuilder.DropTable(
                name: "TBL_Product");

            migrationBuilder.DropTable(
                name: "TBL_Promotion");

            migrationBuilder.DropTable(
                name: "TBL_Role");

            migrationBuilder.DropTable(
                name: "TBL_User");
        }
    }
}
