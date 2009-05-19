using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public abstract class IncludeTypeElement : ConfigurationElement, IIncludeTypeSettings
	{
		private IList<ResponseCompression> _compressionOrderList;
		private const string DEFAULTCACHEFOR = "365:00:00:00.000";
		private const string DEFAULTCOMPRESSIONORDER = "gzip,deflate";
		private const string CACHEFOR = "cacheFor";
		private const string COMPRESSIONORDER = "compressionOrder";
		private const string LINEBREAKAT = "lineBreakAt";
		private const string MINIFY = "minify";
		private const string PATH = "path";

		[ConfigurationProperty(COMPRESSIONORDER, DefaultValue = DEFAULTCOMPRESSIONORDER)]
		private string compressionOrder
		{
			get
			{
				try
				{
					return this[COMPRESSIONORDER] as string;
				}
				catch (NullReferenceException)
				{
					return DEFAULTCOMPRESSIONORDER;
				}
			}
		}

		[ConfigurationProperty(CACHEFOR, DefaultValue = DEFAULTCACHEFOR)]
		private string cacheFor
		{
			get
			{
				try
				{
					return this[CACHEFOR] as string;
				}
				catch (NullReferenceException)
				{
					return DEFAULTCACHEFOR;
				}
			}
		}

		[ConfigurationProperty(LINEBREAKAT, DefaultValue = int.MaxValue)]
		public int LineBreakAt
		{
			get
			{
				int result;
				if (!int.TryParse(this[LINEBREAKAT] as string, out result))
				{
					result = int.MaxValue;
				}
				if (result == 0)
				{
					result = int.MaxValue;
				}
				return result;
			}
		}

		#region IIncludeTypeSettings Members

		[ConfigurationProperty(PATH, DefaultValue = "~/include/{0}/{1}")]
		public string Path
		{
			get { return this[PATH] as string; }
		}

		public IList<ResponseCompression> CompressionOrder
		{
			get
			{
				if (_compressionOrderList == null)
				{
					_compressionOrderList = compressionOrder.Split(',').CastToEnum<ResponseCompression>(true).ToList();
				}
				return _compressionOrderList;
			}
		}

		[ConfigurationProperty(MINIFY, DefaultValue = true)]
		public bool Minify
		{
			get { return (bool) this[MINIFY]; }
		}

		public TimeSpan? CacheFor
		{
			get
			{
				TimeSpan result;
				if (!TimeSpan.TryParse(cacheFor, out result))
				{
					result = TimeSpan.FromDays(365);
				}
				return result;
			}
		}

		#endregion
	}
}