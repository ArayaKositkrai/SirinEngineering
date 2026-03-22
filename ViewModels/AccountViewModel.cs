using System.ComponentModel.DataAnnotations;

namespace SirinEngineering.ViewModels
{
    // Login
    public class LoginViewModel
    {
        [Required(ErrorMessage = "กรุณากรอก Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอก Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    // Register
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "กรุณากรอกชื่อ-นามสกุล")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "กรุณากรอก Username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "กรุณากรอกรหัสผ่าน")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "รหัสผ่านต้องมีอย่างน้อย 6 ตัวอักษร")]
        public string Password { get; set; }

        [Required(ErrorMessage = "กรุณากรอกเบอร์โทรศัพท์")]
        [Phone(ErrorMessage = "รูปแบบเบอร์โทรศัพท์ไม่ถูกต้อง")]
        public string Tel { get; set; }
    }

    // รวมหน้า Login/Register ในหน้าเดียว
    public class AccountViewModel
    {
        public LoginViewModel Login { get; set; } = new LoginViewModel();
        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
        
        // เช็คว่าตอนนี้อยู่ Tab ไหน
        public string ActiveTab { get; set; } = "login"; 
    }
}