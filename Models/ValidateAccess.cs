using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class ValidateAccess
    {
        public int Val_Id { get; set; } // Primary key
        public string Access_Key { get; set; }
        public string Refreshed_Key { get; set; }
        public DateTime ExpireDate { get; set; }
        public DateTime? RenewlyDate { get; set; }
        public string? Admin_Id { get; set; }
        public int? Volunteer_Id { get; set; }
        public int? User_Id { get; set; }
        public String Username { get; set; }

        // Navigation properties
        public Users User { get; set; }
        public Admins Admin { get; set; }
        public Volunteers Volunteer { get; set; }
    }
}
