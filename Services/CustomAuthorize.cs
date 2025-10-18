using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace AttendanceSystem.Services
{
    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Important: Check if session is available
            if (context.HttpContext.Session == null)
            {
                context.Result = new RedirectToPageResult("/AccessDenied");
                return;
            }

            var isLoggedIn = context.HttpContext.Session.GetString("IsLoggedIn");

            var username = context.HttpContext.Session.GetString("Username")
                ?? context.HttpContext.Session.GetString("CurrentUser");

            if (isLoggedIn != "true" || string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToPageResult("/Index");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}