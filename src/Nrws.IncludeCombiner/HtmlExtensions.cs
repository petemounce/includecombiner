using System;
using System.Collections.Generic;
using System.Web.Mvc;

using Microsoft.Practices.ServiceLocation;

namespace Nrws.IncludeCombiner
{
	public static class HtmlExtensions
	{
		public static bool IsInDebugMode(this HtmlHelper helper)
		{
			var debugCookie = helper.ViewContext.HttpContext.Request.Cookies["debug"];
			var trueByCookie = (debugCookie != null && debugCookie.Value == "1" && debugCookie.Expires > DateTime.UtcNow);
			var debugQueryString = helper.ViewContext.HttpContext.Request.QueryString["debug"];
			var trueByQueryString = (debugQueryString != null && debugQueryString == "1");
			return trueByQueryString || trueByCookie;
		}

		public static string RenderIncludes(this HtmlHelper helper, IncludeType type)
		{
			var sources = helper.ViewData[getViewDataKey(type)] as IList<string> ?? new List<string>();
			var combiner = ServiceLocator.Current.GetInstance<IIncludeCombiner>();
			var toRender = combiner.RenderIncludes(sources, type, helper.IsInDebugMode());
			helper.ViewData[getViewDataKey(type)] = new List<string>();
			return toRender;
		}

		public static string RenderCss(this HtmlHelper helper)
		{
			return helper.RenderIncludes(IncludeType.Css);
		}

		public static string RenderScript(this HtmlHelper helper)
		{
			return helper.RenderIncludes(IncludeType.Script);
		}

		public static void Include(this HtmlHelper helper, IncludeType type, string relativePath)
		{
			var relativePathsToInclude =
				helper.ViewData[getViewDataKey(type)] as IList<string> ?? new List<string>();
			if (!relativePathsToInclude.Contains(relativePath))
			{
				relativePathsToInclude.Add(relativePath);
			}
			helper.ViewData[getViewDataKey(type)] = relativePathsToInclude;
		}

		public static void Include(this HtmlHelper helper, IncludeType type, params string[] relativePaths)
		{
			foreach (var path in relativePaths)
			{
				helper.Include(type, path);
			}
		}

		public static void Include(this HtmlHelper helper, IncludeType type, IList<string> relativePaths)
		{
			foreach (var path in relativePaths)
			{
				helper.Include(type, path);
			}
		}

		public static void IncludeCss(this HtmlHelper helper, string relativePath)
		{
			helper.Include(IncludeType.Css, relativePath);
		}

		public static void IncludeCss(this HtmlHelper helper, params string[] relativePaths)
		{
			helper.Include(IncludeType.Css, relativePaths);
		}

		public static void IncludeCss(this HtmlHelper helper, IList<string> relativePaths)
		{
			helper.Include(IncludeType.Css, relativePaths);
		}

		public static void IncludeScript(this HtmlHelper helper, string relativePath)
		{
			helper.Include(IncludeType.Script, relativePath);
		}

		public static void IncludeScript(this HtmlHelper helper, params string[] relativePaths)
		{
			helper.Include(IncludeType.Script, relativePaths);
		}

		public static void IncludeScript(this HtmlHelper helper, IList<string> relativePaths)
		{
			Include(helper, IncludeType.Script, (string[]) relativePaths);
		}

		private static string getViewDataKey(IncludeType type)
		{
			return typeof (HtmlExtensions).FullName + "_" + type;
		}
	}
}