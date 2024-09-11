using ST10157545_GIFTGIVERS.Controllers;
using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Services
{
    // Services/SubscriptionService.cs
    public class SubscriptionService
    {
        private readonly DatabaseController _context;

        public SubscriptionService(DatabaseController context)
        {
            _context = context;
        }

        public async Task<bool> AddSubscriptionAsync(string email)
        {
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
                return false;

            var subscription = new Subscription
            {
                Email = email,
                SubscribedAt = DateTime.Now
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }

}
