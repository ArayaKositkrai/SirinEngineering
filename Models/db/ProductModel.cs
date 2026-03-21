using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db {
    public class ProductModel {
        [Key]
        public int PD_ProductID { get; set; }
        public string PD_ProductName { get; set; }
        public string PD_ProductImage { get; set; }
        public string PD_Brand { get; set; }
        public int PD_CategoryID { get; set; }
        public decimal PD_Price { get; set; }
        public decimal PD_Cost { get; set; }
        public int PD_StockQty { get; set; }
        public int PD_MinStock { get; set; }
        public bool PD_IsActive { get; set; }
    }
}