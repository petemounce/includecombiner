using Yahoo.Yui.Compressor;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface ICssSettings
	{
		CssCompressionType CompressionType { get; }
	}
}