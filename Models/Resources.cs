using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Resources

    {
        [Key]
        public int resource_id { get; set; }

        public string resource_name { get; set; }
        public double resource_quantity { get; set; }
        public string available { get; set; }
        public string managingPerson { get; set; }
        public string categories { get; set; } = "unknown";
        public string projectUsed { get; set; }

        [Required]
        [StringLength(100)]
        public string admin_id { get; set; }

        // Navigation property for Admin
        [ForeignKey("admin_id")]
        public virtual Admins Admin { get; set; }

        //public ICollection<ReliefProject> ReliefProjects { get; set; }


    }
}
