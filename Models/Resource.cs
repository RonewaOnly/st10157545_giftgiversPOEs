using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Models
{
    public class Resource
    {
        public int ResourceId { get; set; }
        public string ResourceName { get; set; }
        public float ResourceQuantity { get; set; }
        public string Available { get; set; }
        public string ManagingPerson { get; set; }
        public string Categories { get; set; } = "unknown";
        public string ProjectUsed { get; set; }
        public string AdminId { get; set; }

        // Navigation property
        public Admins Admin { get; set; }
    }
}
