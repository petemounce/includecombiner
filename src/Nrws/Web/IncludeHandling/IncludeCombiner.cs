using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombiner : IIncludeCombiner
	{
		private static readonly IDictionary<IncludeType, string> _includeFormatStrings = new Dictionary<IncludeType, string>
		{
			{
				IncludeType.Js,
				"<script type='text/javascript' src='{0}'></script>"
				},
			{
				IncludeType.Css,
				"<link rel='stylesheet' type='text/css' href='{0}'/>"
				}
		};

		private readonly IIncludeReader _reader;
		private readonly IKeyGenerator _keyGen;
		private readonly IIncludeStorage _storage;

		public IncludeCombiner(IIncludeReader reader, IKeyGenerator keyGen, IIncludeStorage storage)
		{
			_reader = reader;
			_keyGen = keyGen;
			_storage = storage;
		}

		public string RenderIncludes(ICollection<string> sources, IncludeType type, bool isInDebugMode)
		{
			var toRender = new StringBuilder();
			if (isInDebugMode)
			{
				foreach (var source in sources)
				{
					var url = _reader.ToAbsolute(source);
					toRender.AppendFormat(_includeFormatStrings[type], url).AppendLine();
				}
			}
			else
			{
				var hash = RegisterCombination(sources, type, DateTime.UtcNow);
				var outputUrl = _reader.ToAbsolute(string.Format("~/include/{0}?key={1}", type.ToString().ToLowerInvariant(), HttpUtility.UrlEncode(hash)));
				toRender.AppendFormat(_includeFormatStrings[type], outputUrl);
			}
			return toRender.ToString();
		}

		public string RegisterCombination(ICollection<string> sources, IncludeType type, DateTime now)
		{
			var longKey = new StringBuilder();
			var combinedContent = new StringBuilder();
			foreach (var source in sources)
			{
				longKey.Append("|").Append(source);
				var include = RegisterInclude(source, type);
				combinedContent.Append(include.Content).AppendLine();
			}
			longKey.Remove(0, 1);
			var key = _keyGen.Generate(longKey.ToString());
			var combination = new IncludeCombination(key, type, combinedContent.ToString(), now);
			_storage.Store(combination);
			return key;
		}

		public Include RegisterInclude(string source, IncludeType type)
		{
			var include = _reader.Read(source, type);
			_storage.Store(include);
			return include;
		}

		public IncludeCombination GetCombination(string key)
		{
			return _storage.GetCombination(key);
		}
	}
}