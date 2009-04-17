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

		private readonly ISourceResolver _sourceResolver;

		public IncludeCombiner(ISourceResolver sourceResolver)
		{
			_sourceResolver = sourceResolver;
		}

		public string RenderIncludes(IList<string> sources, IncludeType type, bool isInDebugMode)
		{
			var toRender = new StringBuilder();
			if (isInDebugMode)
			{
				foreach (var source in sources)
				{
					var url = _sourceResolver.Resolve(source);
					toRender.AppendFormat(_includeFormatStrings[type], url).AppendLine();
				}
			}
			else
			{
				var hash = RegisterCombination(type, sources, DateTime.UtcNow);
				var compressorUrl = _sourceResolver.Resolve(string.Format("~/content/compressor/{0}", type));
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