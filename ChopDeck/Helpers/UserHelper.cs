using System.Security.Claims;
using Serilog;

namespace ChopDeck.Helpers
{
    public static class UserHelper
    {
        public static string GetUserId(HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                Log.Warning("User ID is missing from claims. Check authorization");
                return string.Empty;
            }

            return userId;
        }

        public static string GetUserEmail(HttpContext httpContext) {
            var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                Log.Warning("User ID is missing from claims. Check authorization");
                return string.Empty;
            }

            return userEmail;
        }
    }
}
