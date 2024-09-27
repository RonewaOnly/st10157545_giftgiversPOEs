using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using st10157545_giftgiversPOEs.Controllers;
using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Services
{
    public class NotificationService
    {
        private readonly DatabaseController _context;

        public NotificationService(DatabaseController context)
        {
            _context = context;
        }

        // Create a new notification for a user
        public async Task CreateNotificationAsync(string message, string userId)
        {
            var notification = new Notification
            {
                Message = message,
                UserId = userId
            };

            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
            Console.WriteLine($"Notification: {notification}");

        }

        // Fetch unread notifications for a user
        public async Task<List<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _context.Notifications
                                 .Where(n => n.UserId == userId && !n.IsRead)
                                 .ToListAsync();
        }

        // Mark notifications as read
        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }
    }

}
