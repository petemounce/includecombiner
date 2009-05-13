using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling
{
	public interface IIncludeStorage
	{
		void Store(Include include);
		string Store(IncludeCombination combination);
		IncludeCombination GetCombination(string key);
		IEnumerable<Include> GetAllIncludes();
		IDictionary<string, IncludeCombination> GetAllCombinations();
		void Clear();
	}
}