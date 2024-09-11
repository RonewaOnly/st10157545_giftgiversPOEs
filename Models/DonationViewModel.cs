using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class DonationViewModel
    {
        [Required]
        [StringLength(100)]
        public string DonationId { get; set; }

        [Required]
        [StringLength(90)]
        public string ItemName { get; set; }

        [StringLength(30)]
        public string ItemCategory { get; set; }

        [Required]
        public int ItemQuantity { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DonationDate { get; set; }

        [DataType(DataType.Currency)]
        public decimal? CashAmount { get; set; }

        [StringLength(200)]
        public string FullNameDonator { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string AdminId { get; set; }

        public int? VolunteerId { get; set; }
    }
}
