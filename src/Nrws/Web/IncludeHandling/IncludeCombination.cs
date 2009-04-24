using System;
using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombination
	{
		public IncludeCombination(IncludeType type, IEnumerable<string> sources, string content, DateTime now)
		{
			Type = type;
			Sources = sources;
			Content = content;
			LastModifiedAt = now;
		}

		public IncludeType Type { get; protected set; }
		public IEnumerable<string> Sources { get; protected set; }
		public string Content { get; protected set; }
		public DateTime LastModifiedAt { get; protected set; }
	}
}