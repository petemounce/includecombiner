using System;
using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface IIncludeTypeSettings
	{
		int LineBreakAt { get; }
		string Path { get; }
		IList<ResponseCompression> CompressionOrder { get; }
		bool Minify { get; }
		TimeSpan? CacheFor { get; }
	}
}