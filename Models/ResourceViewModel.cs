using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class ResourceViewModel
    {
        public int ResourceId { get; set; }

        [Required]
        [StringLength(120)]
        public string ResourceName { get; set; }

        [Required]
        public float ResourceQuantity { get; set; }

        [Required]
        [StringLength(100)]
        public string Available { get; set; }

        [Required]
        public string ManagingPerson { get; set; }

        [Required]
        [StringLength(90)]
        public string Categories { get; set; } = "unknown";

        [Required]
        public string ProjectUsed { get; set; }

        [Required]
        [StringLength(100)]
        public string AdminId { get; set; }
    }
}
