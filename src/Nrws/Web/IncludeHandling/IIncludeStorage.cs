namespace Nrws.Web.IncludeHandling
{
	public interface IIncludeStorage
	{
		void Store(Include include);
		void Store(IncludeCombination combination);
		IncludeCombination GetCombination(string key);
	}
}