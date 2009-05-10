using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yahoo.Yui.Compressor;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombination : IEquatable<IncludeCombination>
	{
		private readonly string _minified;

		public IncludeCombination(IncludeType type, IEnumerable<string> sources, string content, DateTime now)
		{
			Type = type;
			Sources = sources;
			Content = content;
			LastModifiedAt = now;
			_minified = minify();
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
			return Equals(other._minified, _minified) && Equals(other.Type, Type) && Equals(other.Sources, Sources) && Equals(other.Content, Content) && other.LastModifiedAt.Equals(LastModifiedAt);
		}

		#endregion

		public byte[] GetResponseBodyBytes()
		{
			byte[] responseBodyBytes;
			using (var memoryStream = new MemoryStream(8092))
			{
				var noCompression = Encoding.UTF8.GetBytes(_minified);
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
				var result = (_minified != null ? _minified.GetHashCode() : 0);
				result = (result * 397) ^ Type.GetHashCode();
				result = (result * 397) ^ (Sources != null ? Sources.GetHashCode() : 0);
				result = (result * 397) ^ (Content != null ? Content.GetHashCode() : 0);
				result = (result * 397) ^ LastModifiedAt.GetHashCode();
				return result;
			}
		}

		private string minify()
		{
			if (Content == "")
			{
				return "";
			}
			switch (Type)
			{
				case IncludeType.Js:
					var compressor = new JavaScriptCompressor(Content);
					var minifiedJs = compressor.Compress(false, false, true, false, 80);
					return minifiedJs;

				case IncludeType.Css:
					var minifiedCss = CssCompressor.Compress(Content, int.MaxValue, CssCompressionType.Hybrid);
					return minifiedCss;

				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}