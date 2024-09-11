using Microsoft.AspNetCore.Mvc;
using st10157545_giftgiversPOEs.Services;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string email)
        {
            if (await _subscriptionService.AddSubscriptionAsync(email))
            {
                return Json(new { success = true, message = "Thank you for subscribing!" });
            }

            return Json(new { success = false, message = "Invalid email address." });
        }
    }
}
