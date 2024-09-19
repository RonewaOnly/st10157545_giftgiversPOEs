using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Controllers;

namespace st10157545_giftgiversPOEs.Models
{
    public class SessionService
    {
        private readonly DatabaseController _context;

        public SessionService(DatabaseController context)
        {
            _context = context;
        }

        public async Task<string> GenerateAccessTokenAsync(string username)
        {
            var accessKey = Guid.NewGuid().ToString();
            var expireDate = DateTime.UtcNow.AddMinutes(5); // Set token expiration time to 5 minutes

            var validateAccess = new ValidateAccess
            {
                Access_Key = accessKey,
                Refreshed_Key = accessKey, // Initial refresh key is the same as the access key
                ExpireDate = expireDate,
                Username = username,
            };

            _context.ValidateAccesses.Add(validateAccess);
            await _context.SaveChangesAsync();

            return accessKey;
        }
        public async Task<bool> RefreshAccessTokenAsync(string oldAccessKey)
        {
            var validateAccess = await _context.ValidateAccesses
                .FirstOrDefaultAsync(v => v.Access_Key == oldAccessKey);

            if (validateAccess == null || validateAccess.ExpireDate < DateTime.UtcNow)
            {
                return false; // Token not found or expired
            }

            validateAccess.Refreshed_Key = Guid.NewGuid().ToString();
            validateAccess.RenewlyDate = DateTime.UtcNow;
            validateAccess.ExpireDate = DateTime.UtcNow.AddMinutes(30); // Extend expiration to 30 minutes

            _context.ValidateAccesses.Update(validateAccess);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateTokenAsync(string accessKey)
        {
            var validateAccess = await _context.ValidateAccesses
                .FirstOrDefaultAsync(v => v.Access_Key == accessKey);

            return validateAccess != null && validateAccess.ExpireDate >= DateTime.UtcNow;
        }

        public async Task<bool> InvalidateTokenAsync(string accessKey)
        {
            var validateAccess = await _context.ValidateAccesses
                .FirstOrDefaultAsync(v => v.Access_Key == accessKey);

            if (validateAccess != null)
            {
                _context.ValidateAccesses.Remove(validateAccess);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}
