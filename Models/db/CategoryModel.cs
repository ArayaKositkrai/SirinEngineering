using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SirinEngineering.Models.db
{
    public class CategoryModel
    {
        [Key]
        public int C_CategoryID { get; set; }

        public string C_CategoryName { get; set; }

        [NotMapped]
        public int C_ProductCount { get; set; }
    }
}