using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class ReliefProjectViewModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int relief_id { get; set; }

        [Required]
        [StringLength(120)]
        public string projectName { get; set; }

        public string projectDescription { get; set; }

        [Required]
        [StringLength(200)]
        public string affectedArea { get; set; }

        [Required]
        [StringLength(20)]
        public string teamAssigned { get; set; }

        [Required]
        [StringLength(140)]
        public string teamLeader { get; set; }

        public float totalCost { get; set; }

        [Required]
        public string resourcesUsed { get; set; }

        [Required]
        public DateTime startDate { get; set; }

        public DateTime? endDate { get; set; }

        public string additialTeams { get; set; } = "OPTIONAL";

        public string adminAssignedBy { get; set; }

        public int teamMembers_injured { get; set; }

        // Foreign keys
        [Required]
        [StringLength(100)]
        public string admin_id { get; set; }

        public int? volunteer_id { get; set; }

        public int? resource_id { get; set; }

        // Navigation properties
        public virtual Admins Admin { get; set; }
        public virtual Volunteers Volunteer { get; set; }
        public virtual Resources Resource { get; set; }

        // For dropdowns
        // MultiSelectList for Volunteers, Resources, and Admins
        //public List<int> SelectedVolunteers { get; set; } // Holds the selected volunteer IDs
        //public IEnumerable<SelectListItem> ResourcesList { get; set; }
        //public IEnumerable<SelectListItem> AdminsList { get; set; }
        //public IEnumerable<SelectListItem> VolunteersList { get; set; }


    }
}
