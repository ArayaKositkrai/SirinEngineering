using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SirinEngineering.Models.db {
    public class PromotionModel {
        [Key]
        public int PM_PromotionID { get; set; }
        [Required]
        public string PM_PromotionName { get; set; }
        public string? PM_Detail { get; set; }
        [Required]
        public string PM_PromoType { get; set; }
        public decimal? PM_MinSpend { get; set; }
        public int? PM_MinQuantity { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? PM_DiscountValue { get; set; }
        public int? PM_FreeQuantity { get; set; }
        public decimal? PM_MaxGiftValue { get; set; }
        public int? PM_TargetCategoryID { get; set; }
        [ForeignKey("PM_TargetCategoryID")]
        public virtual CategoryModel TargetCategory { get; set; }
        [Required]
        public DateTime PM_StartDate { get; set; }= DateTime.Now;
        [Required]
        public DateTime PM_EndDate { get; set; }
        public bool PM_IsActive { get; set; } = true;
    }
}