using System;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombination
	{
		public string Key { get; protected set; }
		public IncludeType Type { get; protected set; }
		public string Content { get; protected set; }
		public DateTime LastModifiedAt { get; protected set; }

		public IncludeCombination(string key, IncludeType type, string content, DateTime now)
		{
			Key = key;
			Type = type;
			Content = content;
			LastModifiedAt = now;
		}
	}
}