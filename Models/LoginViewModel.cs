using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "username or email")]
        public string UsernameOrEmail { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

    }
}
