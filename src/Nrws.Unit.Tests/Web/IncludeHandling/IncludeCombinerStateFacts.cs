using System;
using System.Collections.Generic;
using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeCombinerStateFacts
	{
		private readonly IIncludeCombiner _combiner;
		private readonly IIncludeReader _mockReader;
		private readonly MockRepository _mocks;
		private readonly IIncludeStorage _mockStorage;

		public IncludeCombinerStateFacts()
		{
			_mocks = new MockRepository();
			_mockReader = _mocks.DynamicMock<IIncludeReader>();
			_mockStorage = _mocks.DynamicMock<IIncludeStorage>();
			_combiner = new IncludeCombiner(_mockReader, _mockStorage);
			_mocks.ReplayAll();
		}

		public static IEnumerable<object[]> RenderingInDebug
		{
			get
			{
				yield return new object[]
				{
					new Dictionary<string, string> { { "/foo.css", "/foo.css" }, { "/bar.css", "/bar.css" } },
					IncludeType.Css,
					string.Format("<link rel='stylesheet' type='text/css' href='/foo.css'/>{0}<link rel='stylesheet' type='text/css' href='/bar.css'/>{0}", Environment.NewLine)
				};
				yield return new object[]
				{
					new Dictionary<string, string> { { "/foo.js", "/foo.js" }, { "/bar.js", "/bar.js" } },
					IncludeType.Js,
					string.Format("<script type='text/javascript' src='/foo.js'></script>{0}<script type='text/javascript' src='/bar.js'></script>{0}", Environment.NewLine)
				};
				yield return new object[]
				{
					new Dictionary<string, string> { { "~/content/js/foo.js", "/content/js/foo.js" }, { "/bar.js", "/bar.js" } },
					IncludeType.Js,
					string.Format("<script type='text/javascript' src='/content/js/foo.js'></script>{0}<script type='text/javascript' src='/bar.js'></script>{0}", Environment.NewLine)
				};
			}
		}

		public static IEnumerable<object[]> RenderingInRelease
		{
			get
			{
				yield return new object[]
				{
					new Dictionary<string, string> { { "~/content/js/foo.js", "/content/js/foo.js" }, { "/bar.js", "/bar.js" } },
					IncludeType.Js,
					"hashed",
					"<script type='text/javascript' src='/content/js/hashed.js'></script>"
				};
				yield return new object[]
				{
					new Dictionary<string, string> { { "~/content/css/foo.css", "/content/css/foo.css" }, { "/bar.css", "/bar.css" } },
					IncludeType.Css,
					"hashed==",
					"<link rel='stylesheet' type='text/css' href='/content/css/hashed==.css'/>"
				};
				yield return new object[]
				{
					new Dictionary<string, string> { { "~/content/css/foo.css", "/content/css/foo.css" }, { "/bar.css", "/bar.css" } },
					IncludeType.Css,
					"really/nasty%20url=",
					"<link rel='stylesheet' type='text/css' href='/content/css/really/nasty%20url=.css'/>"
				};
			}
		}

		public static IEnumerable<object[]> RegisterCombination
		{
			get
			{
				yield return new object[]
				{
					IncludeType.Js,
					new Dictionary<string, Include>
					{
						{ "~/content/js/foo.js", new Include(IncludeType.Js, "/content/js/foo.js", "alert('hello world!');", Clock.UtcNow) }
					}
				};
			}
		}

		[Fact]
		public void RenderIncludes_ShouldWriteNothing_WhenNoSourcesArePending()
		{
			string rendered = null;
			Assert.DoesNotThrow(() => rendered = _combiner.RenderIncludes(new string[0], IncludeType.Js, false));
			Assert.Equal("", rendered);
		}

		[Theory]
		[PropertyData("RenderingInDebug")]
		public void RenderIncludes_ShouldWriteOutEachIncludeSeparately_WhenInDebugMode(IDictionary<string, string> includes, IncludeType type, string expected)
		{
			foreach (var kvp in includes)
			{
				_mockReader.Expect(sr => sr.ToAbsolute(kvp.Key)).Return(kvp.Value);
			}
			_mockStorage.Expect(s => s.Clear());
			string rendered = null;
			Assert.DoesNotThrow(() => rendered = _combiner.RenderIncludes(includes.Keys, type, true));
			Assert.Equal(rendered, expected);
		}

		[Theory]
		[FreezeClock]
		[PropertyData("RenderingInRelease")]
		public void RenderIncludes_ShouldWriteOutASingleReferenceToTheCompressorController_WhenInReleaseMode(IDictionary<string, string> includes, IncludeType type, string key, string expected)
		{
			foreach (var kvp in includes)
			{
				var include = new Include(type, kvp.Key, "foo", Clock.UtcNow);
				_mockReader.Expect(r => r.Read(kvp.Key, type)).Return(include);
				_mockStorage.Expect(s => s.Store(include));
			}
			_mockReader.Expect(r => r.ToAbsolute(Arg<string>.Is.NotNull)).Return(string.Format("/content/{0}/{1}.{0}", type.ToString().ToLowerInvariant(), key));
			string hash = null;
			_mockStorage.Expect(s => hash = s.Store(Arg<IncludeCombination>.Is.NotNull)).Return("foo");

			string reference = null;
			Assert.DoesNotThrow(() => reference = _combiner.RenderIncludes(includes.Keys, type, false));
			Assert.Equal(expected, reference);
		}

		[Theory]
		[FreezeClock]
		[PropertyData("RegisterCombination")]
		public void RegisterCombination_ShouldReadAllSourcesToAddEachToTheCombination_AndReturnAHash(IncludeType type, IDictionary<string, Include> sources)
		{
			foreach (var kvp in sources)
			{
				_mockReader.Expect(r => r.Read(kvp.Key, kvp.Value.Type)).Return(kvp.Value);
				_mockStorage.Expect(s => s.Store(kvp.Value));
			}
			_mockStorage.Expect(s => s.Store(new IncludeCombination(type, sources.Keys, "content", Clock.UtcNow))).IgnoreArguments().Return("foo");
			string key = null;
			Assert.DoesNotThrow(() => key = _combiner.RegisterCombination(sources.Keys, IncludeType.Js, Clock.UtcNow));
			Assert.Equal("foo", key);
		}
	}
}