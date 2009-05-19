using System.Configuration;
using Yahoo.Yui.Compressor;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public class CssOptionsElement : ConfigurationElement, ICssSettings
	{
		private const string COMPRESSIONTYPE = "compressionType";

		#region ICssSettings Members

		[ConfigurationProperty(COMPRESSIONTYPE, DefaultValue = CssCompressionType.StockYuiCompressor)]
		public CssCompressionType CompressionType
		{
			get
			{
				try
				{
					var type = this[COMPRESSIONTYPE] as string;
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