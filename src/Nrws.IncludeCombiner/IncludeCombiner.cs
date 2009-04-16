using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Nrws.IncludeCombiner
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

		public string RenderIncludes(IList<string> sources, IncludeType type)
		{
			var toRender = new StringBuilder();
			foreach (var source in sources)
			{
				var absolute = VirtualPathUtility.ToAbsolute(source);
				toRender.AppendFormat(_includeFormatStrings[type], absolute).AppendLine();
			}
			return toRender.ToString();
		}
	}
}