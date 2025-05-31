using System.ComponentModel;

namespace EcoSphere.Models
{
    public class UserManagementViewModel
    {
        [DisplayName("User ID")]
        public int UserId { get; set; }
        [DisplayName("User's Name")]
        public string? Name { get; set; }
        [DisplayName("User's Surname")]
        public string? Surname { get; set; }
        [DisplayName("Username")]
        public string? Username { get; set; }
        [DisplayName("E-Mail")]
        public string? Email { get; set; }
        [DisplayName("Role Name")]
        public string? RoleName { get; set; }
        [DisplayName("Creation Date")]
        public DateTime? CreationDate { get; set; }

        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }


    }
}
