using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Volunteers : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int volunteer_id { get; set; }

        [Required]
        [StringLength(129)]
        public string firstname { get; set; }

        [Required]
        [StringLength(150)]
        public string lastname { get; set; }

        [Required]
        [StringLength(12)]
        public string username { get; set; }

        [Required]
        [StringLength(13)]
        [Phone]
        public string phone { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string email { get; set; }

        [StringLength(int.MaxValue)]
        public string skills { get; set; } = "none";

        [Required]
        public int? age { get; set; }

        [StringLength(20)]
        public string gender { get; set; } = "I prefer not to say";

        public int student { get; set; } = 0;

        public string area { get; set; }

        [Required]
        [StringLength(150)]
        public string password { get; set; }
        public UserType UserType { get; } = UserType.Volunteer;

        // Status property
        public bool Status { get; set; } = false; // Default to Inactive

        public virtual ICollection<ReliefProject> ReliefProjects { get; set; }




    }
}
