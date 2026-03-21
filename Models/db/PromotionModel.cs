using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db {
    public class PromotionModel {
        [Key]
        public int PM_PromotionID { get; set; }
        public string PM_PromotionName { get; set; }
        public string PM_Detail { get; set; }
        public string PM_PromoType { get; set; }
        public decimal? PM_MinSpend { get; set; }
        public int? PM_MinQuantity { get; set; }
        public decimal? PM_DiscountValue { get; set; }
        public int? PM_FreeQuantity { get; set; }
        public decimal? PM_MaxGiftValue { get; set; }
        public int? PM_TargetCategoryID { get; set; }
        public DateTime PM_StartDate { get; set; }
        public DateTime PM_EndDate { get; set; }
        public bool PM_IsActive { get; set; }
    }
}