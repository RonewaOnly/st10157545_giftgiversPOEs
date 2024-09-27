using Microsoft.Extensions.Logging;

namespace st10157545_giftgiversPOEs.Models
{
    public class HomeViewModel
    {
        public List<ReliefProject> ReliefProjects { get; set; }
        public List<Events> Events { get; set; } // Assuming you have an Event model
    }

}
