using System;
using System.Web.Mvc;

namespace Nrws.Web
{
	public static class UtilityHtmlExtensions
	{
		public static bool IsInDebugMode(this HtmlHelper helper)
		{
			var debugCookie = helper.ViewContext.HttpContext.Request.Cookies["debug"];
			var trueByCookie = (debugCookie != null && debugCookie.Value == "1" && debugCookie.Expires > DateTime.UtcNow);
			var debugQueryString = helper.ViewContext.HttpContext.Request.QueryString["debug"];
			var trueByQueryString = (debugQueryString != null && debugQueryString == "1");
			return trueByQueryString || trueByCookie;
		}
	}
}