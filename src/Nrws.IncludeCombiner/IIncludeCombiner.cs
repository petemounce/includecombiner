using System.Collections.Generic;

namespace Nrws.IncludeCombiner
{
	public interface IIncludeCombiner
	{
		string RenderIncludes(IList<string> sources, IncludeType type);
	}
}