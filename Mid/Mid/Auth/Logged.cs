using System;
using System.Web;
using System.Web.Mvc;

namespace Mid.Auth
{
    public class Logged : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return httpContext.Session["UserEmail"] != null && httpContext.Session["Password"] != null;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
            {
                controller = "Signup",
                action = "Login"
            }));
        }
    }
}
