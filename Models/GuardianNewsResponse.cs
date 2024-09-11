namespace st10157545_giftgiversPOEs.Models
{
    public class GuardianNewsResponse
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public string Status { get; set; }
        public int Total { get; set; }
        public List<Result> Results { get; set; }
    }

    public class Result
    {
        public string Id { get; set; }
        public string SectionName { get; set; }
        public string WebTitle { get; set; }
        public string WebUrl { get; set; }
        public Fields Fields { get; set; }
    }

    public class Fields
    {
        public string Headline { get; set; }
        public string Thumbnail { get; set; }
        public string BodyText { get; set; }
    }
}
