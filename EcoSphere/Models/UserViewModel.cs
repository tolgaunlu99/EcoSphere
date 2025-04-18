using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace EcoSphere.Models
{
    public class UserViewModel
    {
        [DisplayName("User ID")]
        public int UserId { get; set; }  

        [DisplayName("Name")]
        public string? Name { get; set; }

        [DisplayName("Surname")]
        public string? Surname { get; set; }

        [DisplayName("Username")]
        public string? Username { get; set; }

        [DisplayName("Password")]
        public string? Password { get; set; }

        [DisplayName("Email")]
        public string? Email { get; set; }

        [DisplayName("Mobile Number")]
        public int? MobileNumber { get; set; }

        [DisplayName("Creation Date")]
        public DateTime? CreationDate { get; set; }

        [DisplayName("Updated Date")]
        public DateTime? UpdatedDate { get; set; }

        [DisplayName("Full Name")]
        public string SelectedUserId { get; set; } // Seçilen UserId string olarak gelecek

        public string? FullName => $"{Name} {Surname}";

        [DisplayName("UserNames")]
        public IEnumerable<SelectListItem> UserNamed { get; set; } = new List<SelectListItem>();
    }

}
