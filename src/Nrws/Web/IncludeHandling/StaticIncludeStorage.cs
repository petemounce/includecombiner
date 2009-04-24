using System;
using System.Collections.Generic;
using System.Text;

namespace Nrws.Web.IncludeHandling
{
	public class StaticIncludeStorage : IIncludeStorage
	{
		private static readonly IDictionary<string, IncludeCombination> _combinations;
		private static readonly IDictionary<string, Include> _includes;
		
		static StaticIncludeStorage()
		{
			_includes = new Dictionary<string, Include>();
			_combinations = new Dictionary<string, IncludeCombination>();
		}

		private readonly IKeyGenerator _keyGen;

		public StaticIncludeStorage(IKeyGenerator keyGen)
		{
			_keyGen = keyGen;
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

		public string Store(IncludeCombination combination)
		{
			if (combination == null)
			{
				throw new ArgumentNullException("combination");
			}
			var key = _keyGen.Generate(combination.Sources);

			if (!_combinations.ContainsKey(key))
			{
				_combinations.Add(key, combination);
			}
			else
			{
				_combinations[key] = combination;
			}
			return key;
		}

		public IncludeCombination GetCombination(string key)
		{
			try
			{
				return _combinations[key];
			}
			catch (KeyNotFoundException)
			{
				return null;
			}
		}
	}
}