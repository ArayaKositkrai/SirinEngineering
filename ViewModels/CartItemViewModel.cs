using System.ComponentModel.DataAnnotations;

namespace SirinEngineering.ViewModels
{
    public class CartItemViewModel
    {
        public int CategoryId { get; set; } // เพิ่มบรรทัดนี้เพื่อเอาไว้เช็คโปรหมวดหมู่
        // ไอดีสินค้าสำหรับอ้างอิงในฐานข้อมูล
        public int ProductId { get; set; }

        // ชื่อสินค้า
        public string ProductName { get; set; }

        // ราคาสินค้าต่อชิ้น
        public decimal Price { get; set; }

        // ชื่อไฟล์รูปภาพสินค้า
        public string Image { get; set; }

        // จำนวนที่สั่งซื้อ
        public int Quantity { get; set; }
        // ใช้สำหรับเลือก/ไม่เลือกสินค้านี้ในตะกร้า (ตั้งเป็นไม่เลือกไว้ก่อน)
        public bool IsSelected { get; set; } = false;

        // คำนวณราคารวมของรายการนี้ (ราคา x จำนวน)
        public decimal SubTotal => Price * Quantity;
    }
}