using System.ComponentModel.DataAnnotations;
namespace SirinEngineering.Models.db {
    public class RoleModel {
        [Key]
        public int R_RoleID { get; set; }
        public string R_RoleName { get; set; }
    }
}