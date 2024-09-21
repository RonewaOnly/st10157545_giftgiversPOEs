using Microsoft.AspNetCore.Mvc.Rendering;

namespace st10157545_giftgiversPOEs.Models
{
    public class VolunteerViewModel
    {
        public IEnumerable<Volunteers> Volunteers { get; set; }
        public SelectList ProjectsList { get; set; }
        public SelectList VolunteersList { get; set; }
        public int SelectedVolunteerId { get; set; }
        public int SelectedProjectId { get; set; }
    }
}
