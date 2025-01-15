using ChopDeck.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ValidationActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            Console.WriteLine("Validation errors detected");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }

            context.Result = new BadRequestObjectResult(new ErrorResponse<List<string>>
            {
                Status = 400,
                Message = "One or more validation errors occurred.",
                Data = errors
            });
        }
    }


    public void OnActionExecuted(ActionExecutedContext context) { }
}
