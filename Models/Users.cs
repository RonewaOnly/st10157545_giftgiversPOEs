using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Users : IUser
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int User_id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Firstname { get; set; }

        [Required]
        [MaxLength(150)]
        public string Lastname { get; set; }

        [Required]
        [MaxLength(12)]
        public string username { get; set; }

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MaxLength(12)]
        [Phone]
        public string phone { get; set; }

        [MaxLength(20)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(150)]
        public string password { get; set; }
        public UserType UserType { get; } = UserType.User;

    }
}


