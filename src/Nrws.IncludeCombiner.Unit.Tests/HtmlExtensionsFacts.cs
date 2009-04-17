using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Practices.ServiceLocation;

using Rhino.Mocks;

using Xunit;
using Xunit.Extensions;

namespace Nrws.IncludeCombiner.Unit.Tests
{
	public class HtmlExtensionsFacts
	{
		private readonly HtmlHelper html;
		private readonly HttpContextBase mockHttpContext;
		private readonly MockRepository mocks;
		private readonly IView mockView;
		private readonly ViewContext mockViewContext;
		private readonly ViewDataDictionary viewData;

		public HtmlExtensionsFacts()
		{
			mocks = new MockRepository();

			ServiceLocator.SetLocatorProvider(() => new QnDServiceLocator());

			mockHttpContext = mocks.DynamicMock<HttpContextBase>();
			var cc = mocks.DynamicMock<ControllerContext>(
				mockHttpContext,
				new RouteData(),
				mocks.DynamicMock<ControllerBase>());

			viewData = new ViewDataDictionary();
			mockView = mocks.DynamicMock<IView>();
			mockViewContext = mocks.DynamicMock<ViewContext>(
				cc,
				mockView,
				viewData,
				new TempDataDictionary());

			var mockViewDataContainer = mocks.DynamicMock<IViewDataContainer>();

			mockViewDataContainer.Expect(v => v.ViewData).Return(viewData);

			html = new HtmlHelper(mockViewContext, mockViewDataContainer);
			mocks.ReplayAll();
		}

		public static IEnumerable<object[]> Rendering
		{
			get
			{
				yield return new object[] { IncludeType.Css, new[] { "/foo.css", "/bar.css" }, string.Format("<link rel='stylesheet' type='text/css' href='/foo.css'/>{0}<link rel='stylesheet' type='text/css' href='/bar.css'/>{0}", Environment.NewLine) };
				yield return new object[] { IncludeType.Script, new[] { "/foo.js", "/bar.js" }, string.Format("<script type='text/javascript' src='/foo.js'></script>{0}<script type='text/javascript' src='/bar.js'></script>{0}", Environment.NewLine) };
			}
		}

		public static IEnumerable<object[]> DebugMode
		{
			get
			{
				yield return new object[] { new NameValueCollection(), new HttpCookieCollection(), false };
				yield return new object[] { new NameValueCollection { { "debug", null } }, new HttpCookieCollection(), false };
				yield return new object[] { new NameValueCollection { { "debug", "0" } }, new HttpCookieCollection(), false };
				yield return new object[] { new NameValueCollection { { "debug", "1" } }, new HttpCookieCollection(), true };

				yield return new object[] { new NameValueCollection(), new HttpCookieCollection { new HttpCookie("foo") }, false };
				yield return new object[] { new NameValueCollection(), new HttpCookieCollection { new HttpCookie("debug", "0") }, false };
				yield return new object[] { new NameValueCollection(), new HttpCookieCollection { new HttpCookie("debug", "1") { Expires = DateTime.UtcNow.AddDays(-1) } }, false };
				yield return new object[] { new NameValueCollection(), new HttpCookieCollection { new HttpCookie("debug", "0") { Expires = DateTime.UtcNow.AddDays(1) } }, false };
				yield return new object[] { new NameValueCollection(), new HttpCookieCollection { new HttpCookie("debug", "1") { Expires = DateTime.UtcNow.AddDays(1) } }, true };
			}
		}

		[Theory]
		[PropertyData("DebugMode")]
		public void IsInDebugMode_ShouldBeCorrect(NameValueCollection queryString, HttpCookieCollection cookies, bool expected)
		{
			var mockRequest = mocks.DynamicMock<HttpRequestBase>();
			mockRequest.Expect(r => r.QueryString).Return(queryString);
			mockRequest.Expect(r => r.Cookies).Return(cookies);
			mockRequest.Replay();
			mockViewContext.Expect(vc => vc.View).Return(mockView);
			mockViewContext.Expect(vc => vc.HttpContext).Return(mockHttpContext);
			mockHttpContext.Expect(hc => hc.Request).Return(mockRequest);

			Assert.Equal(expected, html.IsInDebugMode());
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

		[Theory]
		[PropertyData("Rendering")]
		public void Render_ShouldWriteOutEachIncludeSeparately(IncludeType type, IList<string> includes, string expected)
		{
			html.Include(type, includes);
			var rendered = html.RenderIncludes(type);
			Assert.Equal(rendered, expected);
		}

		[Fact]
		public void Rendering_ShouldFlushTheSet()
		{
			html.IncludeCss("/foo.css");
			var before = viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.Equal(1, before.Count);

			html.RenderCss();

			var after = viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.Equal(0, after.Count);
		}

		private static string getViewDataKey(IncludeType type)
		{
			return typeof (HtmlExtensions).FullName + "_" + type;
		}
	}
}