using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Nrws.Web.IncludeCombiner
{
	public class IncludeCombiner : IIncludeCombiner
	{
		private static readonly IDictionary<IncludeType, string> _includeFormatStrings = new Dictionary<IncludeType, string>
		{
			{
				IncludeType.Script,
				"<script type='text/javascript' src='{0}'></script>"
				},
			{
				IncludeType.Css,
				"<link rel='stylesheet' type='text/css' href='{0}'/>"
				}
		};

		public string RenderIncludes(IList<string> sources, IncludeType type, bool isInDebugMode)
		{
			var toRender = new StringBuilder();
			if (isInDebugMode)
			{
				foreach (var source in sources)
				{
					var absolute = VirtualPathUtility.ToAbsolute(source);
					toRender.AppendFormat(_includeFormatStrings[type], absolute).AppendLine();
				}
			}
			else
			{
				var hash = RegisterCombination(type, sources, DateTime.UtcNow);
				var compressorUrl = VirtualPathUtility.ToAbsolute(string.Format("~/content/compressor/{0}", type));
				var outputUrl = string.Format("{0}?hash={1}", compressorUrl, HttpUtility.UrlEncode(hash));
				toRender.AppendFormat(_includeFormatStrings[type], outputUrl);
			}
			return toRender.ToString();
		}

		public string RegisterCombination(IncludeType type, IList<string> sources, DateTime now)
		{
			throw new NotImplementedException();
		}
	}
}