namespace st10157545_giftgiversPOEs.Models
{
    public class TwitterSearchResponse
    {
        public List<Tweet>? Data { get; set; }
        public TwitterIncludes? Includes { get; set; }

        public Meta? Meta { get; set; } // Ensure this is defined in your model

    }

    public class Tweet
    {
        public string? Id { get; set; }
        public string? Text { get; set; }
        public string? CreatedAt { get; set; }
        public string? AuthorId { get; set; }
    }

    public class TwitterIncludes
    {
        public List<TwitterUser>? Users { get; set; }
    }

    public class TwitterUser
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Username { get; set; }
    }

    public class Meta
    {
        public string? NextToken { get; set; }
    }
}
