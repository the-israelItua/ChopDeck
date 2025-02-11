using Microsoft.AspNetCore.Mvc;
using ChopDeck.Models;

namespace ChopDeck.Helpers
{
    public static class ResponseHelper
    {
        public static IActionResult HandleResponse<T>(ApiResponse<T> response)
        {
            return response.Status switch
            {
                201 => new CreatedResult(string.Empty, response),
                401 => new UnauthorizedObjectResult(response),
                500 => new ObjectResult(response) { StatusCode = 500 },
                _ => new OkObjectResult(response),
            };
        }
    }
}
