using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db {
    public class UserModel {
        [Key]
        public int U_UserID { get; set; }
        public string U_Username { get; set; }
        public string U_Password { get; set; }
        public string U_FullName { get; set; }
        public string U_Phone { get; set; }
        public int U_RoleID { get; set; }
        public bool U_IsActive { get; set; }
    }
}