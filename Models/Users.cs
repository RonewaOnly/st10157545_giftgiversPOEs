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

        [StringLength(255)]

        public string? image_url { get; set; } = "https://img.freepik.com/free-vector/illustration-businessman_53876-5856.jpg?t=st=1727442505~exp=1727446105~hmac=9dbba621d4863c1663b0c292b7c1fb4cba2307bf6b265ea3cf04304b1b482a70&w=740";
        [MaxLength(20)]
        public string Gender { get; set; }

        [Required]
        [MaxLength(150)]
        public string password { get; set; }
        public UserType UserType { get; } = UserType.User;

    }
}


