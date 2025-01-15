using ChopDeck.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        context.Result = new ObjectResult(new ErrorResponse<string>
        {
            Status = 500,
            Message = "An unexpected error occurred.",
            Data = context.Exception.Message
        })
        {
            StatusCode = 500
        };
    }
}
