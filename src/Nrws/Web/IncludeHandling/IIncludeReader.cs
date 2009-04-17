namespace Nrws.Web.IncludeHandling
{
	public interface IIncludeReader
	{
		string MapToAbsoluteUri(string source);
		Include Read(string source, IncludeType type);
	}
}