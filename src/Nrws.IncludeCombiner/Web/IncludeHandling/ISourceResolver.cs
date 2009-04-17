namespace Nrws.Web.IncludeHandling
{
	public interface ISourceResolver
	{
		string Resolve(string source);
	}
}