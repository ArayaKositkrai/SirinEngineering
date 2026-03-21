using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db {
    public class PaymentModel {
        [Key]
        public int PY_PaymentID { get; set; }
        public int PY_OrderID { get; set; }
        public DateTime PY_PaymentDate { get; set; }
        public decimal PY_Amount { get; set; }
        public string PY_PaymentSlip { get; set; }
        public string PY_Status { get; set; }
    }
}