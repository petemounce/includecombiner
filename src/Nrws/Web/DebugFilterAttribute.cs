using System;
using System.Web;
using System.Web.Mvc;

namespace Nrws.Web
{
	public class DebugFilterAttribute : FilterAttribute, IActionFilter
	{
		#region IActionFilter Members

		public void OnActionExecuting(ActionExecutingContext filterContext)
		{
			// pass
		}

		public void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var debug = filterContext.HttpContext.Request.QueryString["debug"];
			if (string.IsNullOrEmpty(debug))
			{
				return;
			}

			var existing = filterContext.HttpContext.Response.Cookies["debug"];
			if (debug.Equals("1"))
			{
				if (existing == null)
				{
					filterContext.HttpContext.Response.Cookies.Add(new HttpCookie("debug", "1")
					{
						Domain = filterContext.HttpContext.Request.Url.DnsSafeHost,
						Path = "/",
						HttpOnly = true,
						Expires = DateTime.UtcNow.AddDays(1)
					});
				}
			}
			else if (debug.Equals("0"))
			{
				if (existing != null)
				{
					filterContext.HttpContext.Response.Cookies.Remove("debug");
				}
			}
		}

		#endregion
	}
}