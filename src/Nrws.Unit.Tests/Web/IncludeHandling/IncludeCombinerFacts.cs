using System;
using System.Collections.Generic;
using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeCombinerFacts
	{
		private readonly MockRepository _mocks;
		private readonly IIncludeCombiner _combiner;
		private readonly ISourceResolver _mockSourceResolver;

		public IncludeCombinerFacts()
		{
			_mocks = new MockRepository();
			_mockSourceResolver = _mocks.DynamicMock<ISourceResolver>();
			_combiner = new IncludeCombiner(_mockSourceResolver);
			_mocks.ReplayAll();
		}

		public static IEnumerable<object[]> Rendering
		{
			get
			{
				yield return new object[]
				{
					IncludeType.Css, 
					new Dictionary<string,string> { {"/foo.css", "/foo.css"}, {"/bar.css","/bar.css"} }, 
					true, 
					string.Format("<link rel='stylesheet' type='text/css' href='/foo.css'/>{0}<link rel='stylesheet' type='text/css' href='/bar.css'/>{0}", Environment.NewLine)
				};
				yield return new object[]
				{
					IncludeType.Script, 
					new Dictionary<string, string> { { "/foo.js", "/foo.js" }, { "/bar.js", "/bar.js" } }, 
					true, 
					string.Format("<script type='text/javascript' src='/foo.js'></script>{0}<script type='text/javascript' src='/bar.js'></script>{0}", Environment.NewLine)
				};
			}
		}


		[Theory]
		[PropertyData("Rendering")]
		public void Render_ShouldWriteOutEachIncludeSeparately(IncludeType type, IDictionary<string, string> includes, bool isInDebugMode, string expected)
		{
			foreach (var kvp in includes)
			{
				_mockSourceResolver.Expect(sr => sr.Resolve(kvp.Key)).Return(kvp.Value);
			}
			var rendered = _combiner.RenderIncludes(includes.Keys, type, isInDebugMode);
			Assert.Equal(rendered, expected);
		}

	}
}