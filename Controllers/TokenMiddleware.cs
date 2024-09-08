using Microsoft.EntityFrameworkCore;

namespace ST10157545_GIFTGIVERS.Controllers
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public TokenMiddleware(RequestDelegate next, ILogger<TokenMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<DatabaseController>();
                var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (token != null)
                {
                    var validateAccess = await _context.ValidateAccesses
                        .FirstOrDefaultAsync(v => v.Access_Key == token);

                    if (validateAccess != null)
                    {
                        if (validateAccess.ExpireDate < DateTime.UtcNow)
                        {
                            if (validateAccess.RenewlyDate == null || (DateTime.UtcNow - validateAccess.RenewlyDate.Value).TotalMinutes < 5)
                            {
                                var newAccessKey = Guid.NewGuid().ToString();
                                validateAccess.Access_Key = newAccessKey;
                                validateAccess.Refreshed_Key = newAccessKey;
                                validateAccess.ExpireDate = DateTime.UtcNow.AddMinutes(30);
                                validateAccess.RenewlyDate = DateTime.UtcNow;

                                _context.ValidateAccesses.Update(validateAccess);
                                await _context.SaveChangesAsync();

                                context.Response.Headers.Add("New-Access-Token", newAccessKey);
                            }
                            else
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                return;
                            }
                        }
                    }
                    else
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return;
                    }
                }

                await _next(context);
            }
        }
    }
}
