using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class ReliefProjectViewModel
    {
        public int ReliefId { get; set; }

        [Required]
        [StringLength(120)]
        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        [Required]
        [StringLength(200)]
        public string AffectedArea { get; set; }

        [Required]
        [StringLength(20)]
        public string TeamAssigned { get; set; }

        [Required]
        [StringLength(140)]
        public string TeamLeader { get; set; }

        public float? TotalCost { get; set; }

        [Required]
        public string ResourcesUsed { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        public string AdditionalTeams { get; set; } = "OPTIONAL";

        public string AdminAssignedBy { get; set; }

        public int? TeamMembersInjured { get; set; }

        public int? ResourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string AdminId { get; set; }

        public int? VolunteerId { get; set; }

    }
}
