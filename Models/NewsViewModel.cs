namespace st10157545_giftgiversPOEs.Models
{
    public class NewsViewModel
    {
        public GuardianNewsResponse GuardianNews { get; set; }
        public TwitterSearchResponse TwitterNews { get; set; }
        public InstagramMediaResponse InstagramMedia { get; set; }
        public FacebookPostResponse FacebookPosts { get; set; }

        public Response Response { get; set; }


        // For Pagination
        public string TwitterNextPageToken { get; set; }
        public string InstagramNextPageUrl { get; set; }
        public string FacebookNextPageUrl { get; set; }

        // For Filtering
        public string CountryFilter { get; set; }
    }








}
