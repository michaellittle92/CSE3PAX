using Microsoft.AspNetCore.Mvc.Filters;

namespace CSE3PAX.HelpClasses
{
    public class RequireAuthenticationAttribute : Attribute, IPageFilter
    {
        public string RequiredRole { get; set; } = "";
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            bool isAdmin = context.HttpContext.Session.GetBoolean("isAdmin");
            if (isAdmin == false) { 
            
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {

        }
    }
}
