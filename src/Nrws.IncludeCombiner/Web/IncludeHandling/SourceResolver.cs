using System.Web;

namespace Nrws.Web.IncludeHandling
{
	public class SourceResolver : ISourceResolver
	{
		public string Resolve(string source)
		{
			if (source.StartsWith("http"))
			{
				return source;
			}
			if (source.StartsWith("~"))
			{
				return VirtualPathUtility.ToAbsolute(source);
			}
			// assume absolute path
			return source;
		}
	}
}