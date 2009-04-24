using System.Collections.Generic;

namespace Nrws
{
	public interface IKeyGenerator
	{
		string Generate(IEnumerable<string> generateFrom);
	}
}