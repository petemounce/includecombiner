using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombination : IEquatable<IncludeCombination>
	{
		public IncludeCombination(IncludeType type, IEnumerable<string> sources, string content, DateTime now)
		{
			Type = type;
			Sources = sources;
			Content = content;
			LastModifiedAt = now;
		}

		public IncludeType Type { get; private set; }
		public IEnumerable<string> Sources { get; private set; }
		public string Content { get; private set; }
		public DateTime LastModifiedAt { get; private set; }

		#region IEquatable<IncludeCombination> Members

		public bool Equals(IncludeCombination other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.Type, Type) && Equals(other.Sources, Sources) && Equals(other.Content, Content) && other.LastModifiedAt.Equals(LastModifiedAt);
		}

		#endregion

		public byte[] GetResponseBodyBytes()
		{
			byte[] responseBodyBytes;
			using (var memoryStream = new MemoryStream(8092))
			{
				var noCompression = Encoding.UTF8.GetBytes(Content);
				memoryStream.Write(noCompression, 0, noCompression.Length);
				responseBodyBytes = memoryStream.ToArray();
			}
			return responseBodyBytes;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof (IncludeCombination))
			{
				return false;
			}
			return Equals((IncludeCombination) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var result = Type.GetHashCode();
				result = (result * 397) ^ (Sources != null ? Sources.GetHashCode() : 0);
				result = (result * 397) ^ (Content != null ? Content.GetHashCode() : 0);
				result = (result * 397) ^ LastModifiedAt.GetHashCode();
				return result;
			}
		}
	}
}