using st10157545_giftgiversPOEs.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Donation
    {
        public string DonationId { get; set; }
        public string ItemName { get; set; }
        public string ItemCategory { get; set; }
        public int ItemQuantity { get; set; }
        public DateTime DonationDate { get; set; }
        public decimal? CashAmount { get; set; }
        public string FullnameDonator { get; set; }
        public int? UserId { get; set; }
        public string AdminId { get; set; }
        public int? VolunteerId { get; set; }
        // New foreign key for the event
        public int? EventId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public virtual Users User { get; set; }

        [ForeignKey("AdminId")]
        public virtual Admins Admin { get; set; }

        [ForeignKey("VolunteerId")]
        public virtual Volunteers Volunteer { get; set; }

        [ForeignKey("EventId")]
        public virtual Events Event { get; set; }

    }
}
