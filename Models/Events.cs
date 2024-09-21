using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Events
    {
        [Key]
        public int event_id { get; set; }

        [Required]
        [StringLength(100)]
        public string event_name { get; set; }

        public string description { get; set; }

        [Required]
        [StringLength(255)]
        public string location { get; set; }

        public string image_url { get; set; }

        [Required]
        [StringLength(50)]
        public string event_type { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime event_date { get; set; }

        // This field is populated via the form, and it is required
        [Required]
        [ForeignKey(nameof(Admins))]
        public string admin_id { get; set; }

        // This is a navigation property and should not be validated on form submission
        [BindNever]
        public virtual Admins Admin { get; set; }

        // This is a collection, not validated during form submission
        [BindNever]
        public virtual ICollection<Donation> Donations { get; set; }
    }
}
