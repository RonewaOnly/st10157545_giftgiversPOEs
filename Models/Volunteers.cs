using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ST10157545_GIFTGIVERS.Models
{
    public class Volunteers: IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Volunteer_Id { get; set; }

        [Required]
        [StringLength(129)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(150)]
        public string Lastname { get; set; }

        [Required]
        [StringLength(12)]
        public string Username { get; set; }

        [Required]
        [StringLength(13)]
        [Phone]
        public string Phone { get; set; }

        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(int.MaxValue)]
        public string Skills { get; set; } = "none";

        [Required]
        public int ?Age { get; set; }

        [StringLength(20)]
        public string Gender { get; set; } = "I prefer not to say";

        public int Student { get; set; } = 0;

        public string Area { get; set; }

        [Required]
        [StringLength(150)]
        public string Password { get; set; }
        public UserType UserType { get; } = UserType.Volunteer;

    }
}
