using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    [Table("DONATIONS")]
    public class Donation
    {
        [Key]
        public string donation_id { get; set; } = Guid.NewGuid().ToString(); 
        public string item_name { get; set; }
        public string item_category { get; set; }
        public int item_quantity { get; set; }
        public DateTime donation_date { get; set; }
        public decimal? cash_amount { get; set; }
        public string fullnameDonator { get; set; }

        [ForeignKey(nameof(Users))]
        public int? user_id { get; set; }

        [ForeignKey(nameof(Admins))]
        public string admin_id { get; set; }

        [ForeignKey(nameof(Volunteers))]
        public int? volunteer_id { get; set; }

        [ForeignKey(nameof(Events))]
        [Column("event_id")]
        public int? event_id { get; set; }

        [ForeignKey(nameof(ReliefProject))]

        public int? relief_id { get; set; }

        // Navigation properties
        [BindNever]
        public virtual Users User { get; set; }
        public virtual Admins Admin { get; set; }
        public virtual Volunteers Volunteer { get; set; }
        public virtual Events Event { get; set; }

        public virtual ReliefProject ReliefProject { get; set; }

    }
}
