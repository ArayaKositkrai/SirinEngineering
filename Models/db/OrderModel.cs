using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db
{
    public class OrderModel
    {
        [Key]
        public int O_OrderID { get; set; }
        public DateTime O_OrderDate { get; set; } = DateTime.Now;
        public string O_CustomerName { get; set; }
        public string? O_SellerName { get; set; }
        public int O_ProductID { get; set; }
        public string O_ProductName { get; set; }
        public int O_Quantity { get; set; }
        public decimal O_Price { get; set; }
        public decimal O_SubTotal { get; set; }
        public decimal O_DiscountAmount { get; set; }
        public decimal O_TotalAmount { get; set; }
        public string? O_GiftItemName { get; set; }
        public string O_PaymentType { get; set; }
        public int O_UserID { get; set; }
        public string O_Status { get; set; }
    }
}