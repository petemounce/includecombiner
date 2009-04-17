using System;
using System.Collections.Generic;

namespace Nrws.Web.IncludeCombiner
{
	public interface IIncludeCombiner
	{
		string RenderIncludes(IList<string> sources, IncludeType type, bool isInDebugMode);
		string RegisterCombination(IncludeType type, IList<string> sources, DateTime now);
	}
}