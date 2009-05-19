using System.Configuration;
using Yahoo.Yui.Compressor;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public class CssElement : IncludeTypeElement, ICssSettings
	{
		private const string OPTIONS = "options";

		[ConfigurationProperty(OPTIONS)]
		private CssOptionsElement cssOptions
		{
			get { return (CssOptionsElement) this[OPTIONS] ?? new CssOptionsElement(); }
		}

		#region ICssSettings Members

		public CssCompressionType CompressionType
		{
			get { return cssOptions.CompressionType; }
		}

		#endregion
	}
}