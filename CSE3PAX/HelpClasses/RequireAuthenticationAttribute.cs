using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CSE3PAX.HelpClasses
{
    // AttributeUsage allows the attribute to be used on classes and methods.
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireRolesAttribute : Attribute, IPageFilter
    {
        // Array to hold the roles required for accessing the page.
        private readonly string[] requiredRoles;

        // Constructor that takes an array of roles which are considered authorized for the page.
        public RequireRolesAttribute(params string[] roles)
        {
            this.requiredRoles = roles;
        }

        // This method is part of the IPageFilter interface - I dont think we need it?? maybe 
        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
        }

        // Executed before the page handler method runs.
        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var session = httpContext.Session;
            bool isAuthorized = false; // Flag to determine if the user has any of the required roles.

            // Loop through each required role to check if the user has it.
            foreach (var role in requiredRoles)
            {
                bool? roleStatus = false; // Holds the boolean status of the user's role.
                // Determine which session variable to check based on the role.
                switch (role)
                {
                    case "Admin":
                        roleStatus = session.GetBoolean("isAdministrator");
                        break;
                    case "Manager":
                        roleStatus = session.GetBoolean("isManager");
                        break;
                    case "Lecturer":
                        roleStatus = session.GetBoolean("isLecturer");
                        break;
                }

                // If the user has the role, set isAuthorized to true and break out of the loop.
                if (roleStatus.HasValue && roleStatus.Value)
                {
                    isAuthorized = true;
                    break;
                }
            }

            // If the user is not authorized (does not have any of the required roles), redirect them to index.
            if (!isAuthorized)
            {
                context.Result = new RedirectToPageResult("/Index");
            }
        }

        // This method is part of the IPageFilter interface - I dont think we need it?? maybe 
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }
    }
}
