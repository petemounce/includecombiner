using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Rhino.Mocks;

using Xunit;
using Xunit.Extensions;

namespace Nrws.IncludeCombiner.Unit.Tests
{
	public class HtmlExtensionsFacts
	{
		private readonly HtmlHelper html;
		private readonly MockRepository mocks;
		private readonly ViewDataDictionary viewData;

		public HtmlExtensionsFacts()
		{
			mocks = new MockRepository();

			var cc = mocks.DynamicMock<ControllerContext>(
				mocks.DynamicMock<HttpContextBase>(),
				new RouteData(),
				mocks.DynamicMock<ControllerBase>());

			viewData = new ViewDataDictionary();
			var mockViewContext = mocks.DynamicMock<ViewContext>(
				cc,
				mocks.DynamicMock<IView>(),
				viewData,
				new TempDataDictionary());

			var mockViewDataContainer = mocks.DynamicMock<IViewDataContainer>();

			mockViewDataContainer.Expect(v => v.ViewData).Return(viewData);

			html = new HtmlHelper(mockViewContext, mockViewDataContainer);
			mocks.ReplayAll();
		}

		[Fact]
		public void AddInclude_ShouldAppendIncludeToSetInViewData()
		{
			html.IncludeCss("~/content/css/site.css");

			var set = viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.NotNull(set);
			Assert.Equal(1, set.Count);
			Assert.Equal("~/content/css/site.css", set[0]);
		}

		[Fact]
		public void AddMultipleIncludes_ShouldAppendIncludeToSetInSameOrderAsAdded()
		{
			html.IncludeScript("foo");
			html.IncludeScript("bar");
			html.IncludeScript("baz");

			var set = viewData[getViewDataKey(IncludeType.Script)] as IList<string>;
			Assert.NotNull(set);
			Assert.Equal(3, set.Count);
			Assert.Equal("foo", set[0]);
			Assert.Equal("bar", set[1]);
			Assert.Equal("baz", set[2]);
		}

		[Fact]
		public void AddSameIncludeMoreThanOnce_ShouldOnlyAddIncludeTheFirstTime()
		{
			html.IncludeScript("foo");
			html.IncludeScript("bar");
			html.IncludeScript("foo");

			var set = viewData[getViewDataKey(IncludeType.Script)] as IList<string>;
			Assert.NotNull(set);
			Assert.Equal(2, set.Count);
			Assert.Equal("foo", set[0]);
			Assert.Equal("bar", set[1]);
		}

		[Fact]
		public void AddIncludesOfDifferentTypes_ShouldAddToAppropriateSet()
		{
			html.IncludeScript("foo.js");
			html.IncludeCss("foo.css");

			var jsSet = viewData[getViewDataKey(IncludeType.Script)] as IList<string>;
			Assert.NotNull(jsSet);
			Assert.Equal(1, jsSet.Count);
			Assert.Equal("foo.js", jsSet[0]);

			var cssSet = viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.NotNull(cssSet);
			Assert.Equal(1, cssSet.Count);
			Assert.Equal("foo.css", cssSet[0]);
		}

		public static IEnumerable<object[]> Rendering
		{
			get
			{
				yield return new object[] { IncludeType.Css, new[] { "/foo.css", "/bar.css" }, string.Format("<link rel='stylesheet' type='text/css' href='/foo.css' />{0}<link rel='stylesheet' type='text/css' href='/bar.css' />{0}", Environment.NewLine) };
				yield return new object[] { IncludeType.Script, new[] { "/foo.js", "/bar.js" }, string.Format("<script type='text/javascript' src='/foo.js'></script>{0}<script type='text/javascript' src='/bar.js'></script>{0}", Environment.NewLine) };
			}
		}

		[Theory]
		[PropertyData("Rendering")]
		public void Render_ShouldWriteOutEachIncludeSeparately(IncludeType type, IList<string> includes, string expected)
		{
			html.Include(type, includes);
			var rendered = html.RenderIncludes(type);
			Assert.Equal(rendered, expected);
		}

		private static string getViewDataKey(IncludeType type)
		{
			return typeof (HtmlExtensions).FullName + "_" + type;
		}
	}
}