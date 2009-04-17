using System;
using System.IO;

namespace Nrws.Web.IncludeCombiner
{
	public class FileSystemIncludeReader : IIncludeReader
	{
		public string Read(string source)
		{
			if (String.IsNullOrEmpty(source))
				throw new ArgumentException("source must have a value", source);

			var file = new FileInfo(source);
			if (!file.Exists)
				throw new InvalidOperationException(string.Format("{0} does not exist", source));

			return File.ReadAllText(source);
		}
	}
}