using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SASA.Filters
{
    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var email = session.GetString("auth_email");

            if (string.IsNullOrEmpty(email))
            {
                context.Result = new RedirectResult("/login");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
