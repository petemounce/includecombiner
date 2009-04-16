using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Nrws.IncludeCombiner
{
	public static class HtmlExtensions
	{
		private static readonly IDictionary<IncludeType, string> _compressorUrlFormatStrings = new Dictionary<IncludeType, string>
		{
			{
				IncludeType.Script,
				"<script type='text/javascript' src='{0}'></script>"
				},
			{
				IncludeType.Css,
				"<link rel='stylesheet' type='text/css' href='{0}' />"
				}
		};

		public static string RenderIncludes(this HtmlHelper helper, IncludeType type)
		{
			var relativePathsToInclude = helper.ViewData[getViewDataKey(type)] as IList<string> ?? new List<string>();
			var toRender = new StringBuilder();
			foreach (var path in relativePathsToInclude)
			{
				var absolute = VirtualPathUtility.ToAbsolute(path);
				toRender.AppendFormat(_compressorUrlFormatStrings[type], absolute).AppendLine();
			}
			helper.ViewData[getViewDataKey(type)] = new List<string>();
			return toRender.ToString();
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