namespace st10157545_giftgiversPOEs.Models
{
    public class InstagramMediaResponse
    {
        public List<InstagramMedia> Data { get; set; }
        public Paging Paging { get; set; } // Ensure this is defined in your model

    }

    public class InstagramMedia
    {
        public string Id { get; set; }
        public string Caption { get; set; }
        public string MediaUrl { get; set; }
        public string Timestamp { get; set; }
    }
    public class Paging
    {
        public string Next { get; set; }
    }
}
