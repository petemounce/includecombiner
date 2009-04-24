using System;
using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling
{
	public interface IIncludeCombiner
	{
		string RenderIncludes(ICollection<string> sources, IncludeType type, bool isInDebugMode);
		string RegisterCombination(ICollection<string> sources, IncludeType type, DateTime now);
		Include RegisterInclude(string source, IncludeType type);
		IncludeCombination GetCombination(string key);
	}
}