using System;
using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling
{
	public class StaticIncludeStorage : IIncludeStorage
	{
		private static IDictionary<string, Include> _includes;
		private static IDictionary<string, IncludeCombination> _combinations;

		public StaticIncludeStorage()
		{
			_includes = new Dictionary<string, Include>();
			_combinations = new Dictionary<string, IncludeCombination>();
		}

		public void Store(Include include)
		{
			if (include == null)
			{
				throw new ArgumentNullException("include");
			}
			if (!_includes.ContainsKey(include.Uri))
			{
				_includes.Add(include.Uri, include);
			}
			else
			{
				_includes[include.Uri] = include;
			}
		}

		public void Store(IncludeCombination combination)
		{
			if (combination == null)
			{
				throw new ArgumentNullException("combination");
			}
			if (!_combinations.ContainsKey(combination.Key))
			{
				_combinations.Add(combination.Key, combination);
			}
			else
			{
				_combinations[combination.Key] = combination;
			}
		}
	}
}