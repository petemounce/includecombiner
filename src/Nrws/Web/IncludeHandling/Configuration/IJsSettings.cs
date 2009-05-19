namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface IJsSettings
	{
		bool Verbose { get; }
		bool Obfuscate { get; }
		bool PreserveSemiColons { get; }
		bool DisableOptimizations { get; }
	}
}