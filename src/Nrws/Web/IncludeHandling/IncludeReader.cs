using System;
using System.IO;
using System.Web;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeReader : IIncludeReader
	{
		public string MapToAbsoluteUri(string source)
		{
			if (source.StartsWith("~"))
			{
				return VirtualPathUtility.ToAbsolute(source);
			}
			// assume absolute path or external resource
			return source;
		}

		public Include Read(string source, IncludeType type)
		{
			if (String.IsNullOrEmpty(source))
				throw new ArgumentException("source must have a value", source);

			var file = new FileInfo(source);
			if (!file.Exists)
				throw new InvalidOperationException(string.Format("{0} does not exist", source));

			var content = File.ReadAllText(source);
			var lastModifiedAt = File.GetLastWriteTimeUtc(source);
			return new Include(type, source, content, lastModifiedAt);
		}
	}
}