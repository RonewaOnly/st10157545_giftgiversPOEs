using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Models
{
    public class ReliefProject
    {
        public int relief_id { get; set; }
        public string projectName { get; set; }
        public string projectDescription { get; set; }
        public string affectedArea { get; set; }
        public string teamAssigned { get; set; }
        public string teamLeader { get; set; }
        public float? totalCost { get; set; }
        public string resourcesUsed { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string additialTeams { get; set; } = "OPTIONAL";
        public string adminAssignedBy { get; set; }
        public int? teamMembers_injured { get; set; }
        public int? resource_id { get; set; }
        public string admin_id { get; set; }
        public int? volunteer_id { get; set; }

        // Navigation properties
        public Resource Resource { get; set; }
        public Admins Admin { get; set; }
        public Volunteers Volunteer { get; set; }
    }
}
