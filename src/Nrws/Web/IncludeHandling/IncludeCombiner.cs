using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Nrws.Web.IncludeHandling.Configuration;

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

		private readonly IIncludeHandlingSettings _settings;
		private readonly IIncludeReader _reader;
		private readonly IIncludeStorage _storage;

		public IncludeCombiner(IIncludeHandlingSettings settings, IIncludeReader reader, IIncludeStorage storage)
		{
			_settings = settings;
			_reader = reader;
			_storage = storage;
		}

		#region IIncludeCombiner Members

		public string RenderIncludes(ICollection<string> sources, IncludeType type, bool isInDebugMode)
		{
			var toRender = new StringBuilder();
			if (sources.Count > 0)
			{
				if (_settings.AllowDebug && isInDebugMode)
				{
					Clear();
					foreach (var source in sources)
					{
						var url = _reader.ToAbsolute(source);
						toRender.AppendFormat(_includeFormatStrings[type], url).AppendLine();
					}
				}
				else
				{
					var hash = RegisterCombination(sources, type, DateTime.UtcNow);
					var outputUrl = _reader.ToAbsolute(string.Format(_settings.Types[type].Path, type.ToString().ToLowerInvariant(), HttpUtility.UrlEncode(hash)));
					toRender.AppendFormat(_includeFormatStrings[type], outputUrl);
				}
			}
			return toRender.ToString();
		}

		public string RegisterCombination(ICollection<string> sources, IncludeType type, DateTime now)
		{
			var combinedContent = new StringBuilder();
			foreach (var source in sources)
			{
				var include = RegisterInclude(source, type);
				combinedContent.Append(include.Content).AppendLine();
			}
			var combination = new IncludeCombination(type, sources, combinedContent.ToString(), now);
			var key = _storage.Store(combination);
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

		public IEnumerable<Include> GetAllIncludes()
		{
			return _storage.GetAllIncludes();
		}

		public IDictionary<string, IncludeCombination> GetAllCombinations()
		{
			return _storage.GetAllCombinations();
		}

		public void UpdateCombination(IncludeCombination combination)
		{
			_storage.Store(combination);
		}

		public void Clear()
		{
			_storage.Clear();
		}

		#endregion
	}
}