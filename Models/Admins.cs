using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Admins : IUser
    {
        [Key]
        [StringLength(100)]
        public string admin_id { get; set; }

        [Required]
        [StringLength(120)]
        public string? firstname { get; set; }

        [Required]
        [StringLength(150)]
        public string? lastname { get; set; }

        [Required]
        [StringLength(20)]
        public string? username { get; set; }


        [Required]
        [StringLength(13)]
        [Phone]
        public string? phone { get; set; }

        [StringLength(20)]
        public string? gender { get; set; } = "I prefer not to say";

        [Required]
        public string? qualifications { get; set; }

        public string? skills { get; set; }

        public string? speciality { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string email { get; set; }

        [Required]
        [StringLength(255)]
        public string password { get; set; }

        public UserType UserType { get; } = UserType.Admin;

        public virtual ICollection<Events> Events { get; set; }
        public ICollection<ReliefProject> ReliefProjects { get; set; }
        public virtual ICollection<Resources> Resources { get; set; }

    }
}
