using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Website_Plant.MyHelpers
{
    public class RequireAuthAttribute : Attribute, IPageFilter
    {
        public string RequiredRole { get; set; } = "";
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            string? role = context.HttpContext.Session.GetString("role");

            if (role == null)
            {
                context.Result = new RedirectResult("/");
            }
            else
            {
                if (RequiredRole.Length > 0 && !RequiredRole.Equals(role))
                {
                    context.Result = new RedirectResult("/");
                }
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }
    }
}
