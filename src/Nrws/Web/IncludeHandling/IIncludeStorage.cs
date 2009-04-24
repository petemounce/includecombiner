namespace Nrws.Web.IncludeHandling
{
	public interface IIncludeStorage
	{
		void Store(Include include);
		string Store(IncludeCombination combination);
		IncludeCombination GetCombination(string key);
	}
}