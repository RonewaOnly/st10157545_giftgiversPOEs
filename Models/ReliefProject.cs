using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class ReliefProject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int relief_id { get; set; }

        [Required]
        [StringLength(120)]
        public string projectName { get; set; }

        public string? projectDescription { get; set; }

        [Required]
        [StringLength(200)]
        public string affectedArea { get; set; }

        [StringLength(20)]
        public string? teamAssigned { get; set; }

        [Required]
        [StringLength(140)]
        public string? teamLeader { get; set; }

        public double totalCost { get; set; }

        [Required]
        public string resourcesUsed { get; set; } = "nothing";

        [Required]
        public DateOnly? startDate { get; set; }

        public DateOnly? endDate { get; set; }

        public string? additialTeams { get; set; } = "OPTIONAL";

        public string? adminAssignedBy { get; set; } = "OPTIONAL";

        public int? teamMembers_injured { get; set; }

        // Foreign keys
        [Required]
        [StringLength(100)]
        public string admin_id { get; set; }
        public int? volunteer_id { get; set; }
        public int? resource_id { get; set; }

        [ForeignKey("admin_id")]
        public virtual Admins Admin { get; set; }

        [ForeignKey("volunteer_id")]
        public virtual Volunteers Volunteer { get; set; }

        [ForeignKey("resource_id")]
        public virtual Resources Resource { get; set; }

        //public virtual ICollection<Volunteers> Volunteers { get; set; } = new List<Volunteers>();

    }
}
