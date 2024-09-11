using Microsoft.AspNetCore.Mvc;
using st10157545_giftgiversPOEs.Models;
using st10157545_giftgiversPOEs.Services;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly GuardianNewsService _newsService;
        private readonly TwitterService _twitterService;
        private readonly InstagramService _instagramService;
        private readonly FacebookService _facebookService;
        public VolunteerController(GuardianNewsService newsService, TwitterService twitterService, InstagramService instagramService, FacebookService facebookService)
        {
            _newsService = newsService;
            _twitterService = twitterService;
            _instagramService = instagramService;
            _facebookService = facebookService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Events()
        {
            return View();
        }
        
        public async Task<IActionResult> News(string countryFilter = null, string twitterNextToken = null, string instagramNextPage = null, string facebookNextPage = null)
        {
            var news = await _newsService.GetLatestNewsAsync();
            var viewModel = new NewsViewModel();

            // Fetch filtered and paginated data
            viewModel.GuardianNews = news;
            viewModel.TwitterNews = await _twitterService.SearchDisasterNewsAsync(country: countryFilter, nextToken: twitterNextToken);
            viewModel.InstagramMedia = await _instagramService.GetDisasterMediaAsync(nextPageUrl: instagramNextPage);
            viewModel.FacebookPosts = await _facebookService.GetDisasterPostsAsync(nextPageUrl: facebookNextPage);

            // Store the next page tokens for pagination
            viewModel.TwitterNextPageToken = viewModel.TwitterNews?.Meta?.NextToken;
            viewModel.InstagramNextPageUrl = viewModel.InstagramMedia?.Paging?.Next;
            viewModel.FacebookNextPageUrl = viewModel.FacebookPosts?.Paging?.Next;

            //var model = new NewsViewModel
            //{
            //    GuardianNews = news,
            //    TwitterNews = twitterNews,
            //    InstagramMedia = instagramMedia,
            //    FacebookPosts = facebookPosts
            //};
            return View(viewModel);
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Tasks()
        {
            return View();
        }
    }
}
