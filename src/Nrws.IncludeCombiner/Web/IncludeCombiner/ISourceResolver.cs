namespace Nrws.Web.IncludeCombiner
{
	public interface ISourceResolver
	{
		string Resolve(string source);
	}
}