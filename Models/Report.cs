using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace st10157545_giftgiversPOEs.Models
{
    public class Report
    {
        [Key]
        public int id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column("report_id", TypeName = "varchar(50)")]
        public string report_id { get; set; }

        [ForeignKey("Users")]
        [Column("user_id")]
        public int? user_id { get; set; }

        [Column("is_anonymous")]
        public bool is_anonymous { get; set; } = false;

        [Required]
        [StringLength(50)]
        [Column("disaster_type")]
        public string disaster_type { get; set; }

        [Required]
        [Column("description")]
        public string description { get; set; }

        [Required]
        [StringLength(255)]
        [Column("location")]
        public string location { get; set; }

        [DataType(DataType.DateTime)]
        [Column("report_date")]
        public DateTime report_date { get; set; } = DateTime.Now;

        [Column("severity_level")]
        public int? severity_level { get; set; }

        [StringLength(255)]
        [Column("image_url")]
        public string image_url { get; set; }

        [StringLength(20)]
        [Column("status")]
        public string status { get; set; } = "Pending";

        public Users User { get; set; }
    }

}
