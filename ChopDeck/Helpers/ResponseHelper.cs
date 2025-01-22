using Microsoft.AspNetCore.Mvc;
using ChopDeck.Models;

namespace ChopDeck.Helpers
{
    public static class ResponseHelper
    {
        public static IActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            switch (response.Status)
            {
                case 401:
                    return new UnauthorizedObjectResult(response);

                case 500:
                    return new ObjectResult(response) { StatusCode = 500 };

                default:
                    return new OkObjectResult(response);
            }
        }
    }
}
