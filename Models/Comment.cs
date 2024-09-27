using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace st10157545_giftgiversPOEs.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Report")]
        public int report_id { get; set; }

        [ForeignKey("Users")]
        public int? user_id { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Report Report { get; set; }
        public virtual Users User { get; set; }
    }

}
