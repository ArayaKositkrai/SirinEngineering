using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db {
    public class CategoryModel {
        [Key]
        public int C_CategoryID { get; set; }
        public string C_CategoryName { get; set; }
    }
}