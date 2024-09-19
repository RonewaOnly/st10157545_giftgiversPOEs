using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(120)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required]
        [Phone]
        [StringLength(13)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [StringLength(20)]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = "I prefer not to say";

        [Required]
        [StringLength(255, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }

        // Additional fields for Admin and Volunteer
        [Display(Name = "Qualifications")]
        public string? Qualifications { get; set; }

        [Display(Name = "Skills")]
        public string? Skills { get; set; }

        [Display(Name = "Speciality")]
        public string? Speciality { get; set; }

        [Display(Name = "Age")]
        public int? Age { get; set; }

        [Display(Name = "Is Student")]
        public bool IsStudent { get; set; }

        [Display(Name = "Area")]
        public string? Area { get; set; }

        [Display(Name = "User Type")]
        [Required]
        public UserType UserType { get; set; }
    }
    public enum UserType
    {
        User,
        Admin,
        Volunteer
    }
}
