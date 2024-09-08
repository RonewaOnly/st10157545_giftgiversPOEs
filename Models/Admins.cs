using System.ComponentModel.DataAnnotations;

namespace ST10157545_GIFTGIVERS.Models
{
    public class Admins : IUser
    {
        [Key]
        [StringLength(100)]
        public string Admin_Id { get; set; }

        [Required]
        [StringLength(120)]
        public string? Firstname { get; set; }

        [Required]
        [StringLength(150)]
        public string? Lastname { get; set; }

        [Required]
        [StringLength(20)]
        public  string? Username { get; set; }


        [Required]
        [StringLength(13)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(20)]
        public string? Gender { get; set; } = "I prefer not to say";

        [Required]
        public string? Qualifications { get; set; }

        public string? Skills { get; set; }

        public string? Speciality { get; set; }

        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        public UserType UserType { get; } = UserType.Admin;

    }
}
