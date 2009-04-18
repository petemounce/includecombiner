using System;

namespace Nrws.Web.IncludeHandling
{
	public enum IncludeType
	{
		Js,
		Css
	}

	public class Include
	{
		public Include(IncludeType type, string uri, string content, DateTime lastModifiedAt)
		{
			Type = type;
			Uri = uri;
			Content = content;
			LastModifiedAt = lastModifiedAt;
		}

		public IncludeType Type { get; protected set; }
		public string Uri { get; protected set; }
		public string Content { get; protected set; }
		public DateTime LastModifiedAt { get; protected set; }
	}
}