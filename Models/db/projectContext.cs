using Microsoft.EntityFrameworkCore;
using SirinEngineering.Models.db;

namespace SirinEngineering.Models.db
{
    public class projectContext : DbContext
    {
        public projectContext(DbContextOptions<projectContext> options) : base(options) { }

        public DbSet<RoleModel> TBL_Role { get; set; }
        public DbSet<CategoryModel> TBL_Category { get; set; }
        public DbSet<UserModel> TBL_User { get; set; }
        public DbSet<ProductModel> TBL_Product { get; set; }
        public DbSet<PromotionModel> TBL_Promotion { get; set; }
        public DbSet<OrderModel> TBL_Order { get; set; }
        public DbSet<PaymentModel> TBL_Payment { get; set; }
    }
}