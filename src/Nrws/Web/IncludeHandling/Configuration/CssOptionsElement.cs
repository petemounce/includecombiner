using System.Configuration;
using Yahoo.Yui.Compressor;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public class CssOptionsElement : ConfigurationElement, ICssMinifySettings
	{
		private const string COMPRESSIONTYPE = "compressionType";

		#region ICssMinifySettings Members

		[ConfigurationProperty(COMPRESSIONTYPE, DefaultValue = CssCompressionType.StockYuiCompressor)]
		public CssCompressionType CompressionType
		{
			get
			{
				try
				{
					var type = this[COMPRESSIONTYPE].ToString();
					return type.CastToEnum<CssCompressionType>();
				}
				catch
				{
					return CssCompressionType.StockYuiCompressor;
				}
			}
		}

		#endregion
	}
}