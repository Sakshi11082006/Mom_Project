using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Mom_Project.Filters
{
    public class CheckAccess : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>();

            // ✅ Allow all anonymous actions (Login GET + POST)
            if (allowAnonymous != null)
            {
                return;
            }

            var user = context.HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(user))
            {
                context.Result = new RedirectToActionResult("Login", "UserLogin", null);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
