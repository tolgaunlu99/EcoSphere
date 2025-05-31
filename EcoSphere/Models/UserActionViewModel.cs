namespace EcoSphere.Models
{
    public class UserActionViewModel
    {
        public int UserActionID { get; set; }
        public string? UserName { get; set; }
        public string? Action { get; set; }
        public DateTime ActionTime { get; set; }
    }
}
