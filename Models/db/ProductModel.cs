using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SirinEngineering.Models.db {
    public class ProductModel {
        [Key]
        public int PD_ProductID { get; set; }
        public string PD_ProductName { get; set; }
        public string PD_ProductImage { get; set; }
        public string PD_Brand { get; set; }
        public int PD_CategoryID { get; set; }
        [ForeignKey("PD_CategoryID")]
        public virtual CategoryModel Category { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "ราคาห้ามติดลบ")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PD_Price { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "ราคาต้นทุนห้ามติดลบ")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal PD_Cost { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "จำนวนสต็อกห้ามติดลบ")]
        public int PD_StockQty { get; set; }
        public int PD_MinStock { get; set; }
        public bool PD_IsActive { get; set; } = true;
    }
}