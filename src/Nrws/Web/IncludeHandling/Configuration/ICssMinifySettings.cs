using Yahoo.Yui.Compressor;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface ICssMinifySettings
	{
		CssCompressionType CompressionType { get; }
	}
}