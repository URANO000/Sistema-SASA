using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SASA.Filters
{
    public class RequireAuthAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isAuth = context.HttpContext.User?.Identity?.IsAuthenticated ?? false;

            if (!isAuth)
            {
                context.Result = new RedirectResult("/login");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
