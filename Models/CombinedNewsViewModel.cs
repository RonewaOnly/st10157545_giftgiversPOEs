namespace st10157545_giftgiversPOEs.Models
{
    public class CombinedNewsViewModel
    {
        // Guardian News
        public GuardianNewsResponse? GuardianNews { get; set; }

        // Social Media News
        public TwitterSearchResponse? TwitterNews { get; set; }
        public InstagramMediaResponse? InstagramMedia { get; set; }
        public FacebookPostResponse? FacebookPosts { get; set; }

        // Pagination tokens
        public string? TwitterNextPageToken { get; set; }
        public string? InstagramNextPageUrl { get; set; }
        public string? FacebookNextPageUrl { get; set; }

        // Filtering options
        public string? CountryFilter { get; set; }

        // Any additional properties from MediaNewsViewModelCombined
        //public string SomeOtherProperty { get; set; } // Example
        public Response? Response { get; set; }

    }
}
