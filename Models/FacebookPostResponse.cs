namespace st10157545_giftgiversPOEs.Models
{
    public class FacebookPostResponse
    {
        public List<FacebookPost> Data { get; set; }
        public Paging Paging { get; set; } // Ensure this is defined in your model

    }

    public class FacebookPost
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public string CreatedTime { get; set; }
    }

}
