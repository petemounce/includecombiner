using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeCombinerHtmlExtensionsFacts
	{
		private readonly HtmlHelper _html;
		private readonly MockRepository _mocks;
		private readonly ViewDataDictionary _viewData;

		public IncludeCombinerHtmlExtensionsFacts()
		{
			_mocks = new MockRepository();

			ServiceLocator.SetLocatorProvider(() => new QnDServiceLocator());

			_viewData = new ViewDataDictionary();

			_html = WebTestUtility.BuildHtmlHelper(_mocks, _viewData, null);
			_mocks.ReplayAll();
		}

		[Fact]
		public void AddInclude_ShouldAppendIncludeToSetInViewData()
		{
			_html.IncludeCss("~/content/css/site.css");

			var set = _viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.NotNull(set);
			Assert.Equal(1, set.Count);
			Assert.Equal("~/content/css/site.css", set[0]);
		}

		[Fact]
		public void AddMultipleIncludes_ShouldAppendIncludeToSetInSameOrderAsAdded()
		{
			_html.IncludeScript("foo");
			_html.IncludeScript("bar");
			_html.IncludeScript("baz");

			var set = _viewData[getViewDataKey(IncludeType.Script)] as IList<string>;
			Assert.NotNull(set);
			Assert.Equal(3, set.Count);
			Assert.Equal("foo", set[0]);
			Assert.Equal("bar", set[1]);
			Assert.Equal("baz", set[2]);
		}

		[Fact]
		public void AddSameIncludeMoreThanOnce_ShouldOnlyAddIncludeTheFirstTime()
		{
			_html.IncludeScript("foo");
			_html.IncludeScript("bar");
			_html.IncludeScript("foo");

			var set = _viewData[getViewDataKey(IncludeType.Script)] as IList<string>;
			Assert.NotNull(set);
			Assert.Equal(2, set.Count);
			Assert.Equal("foo", set[0]);
			Assert.Equal("bar", set[1]);
		}

		[Fact]
		public void AddIncludesOfDifferentTypes_ShouldAddToAppropriateSet()
		{
			_html.IncludeScript("foo.js");
			_html.IncludeCss("foo.css");

			var jsSet = _viewData[getViewDataKey(IncludeType.Script)] as IList<string>;
			Assert.NotNull(jsSet);
			Assert.Equal(1, jsSet.Count);
			Assert.Equal("foo.js", jsSet[0]);

			var cssSet = _viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.NotNull(cssSet);
			Assert.Equal(1, cssSet.Count);
			Assert.Equal("foo.css", cssSet[0]);
		}

		[Fact]
		public void Rendering_ShouldFlushTheSet()
		{
			_html.IncludeCss("/foo.css");
			var before = _viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.Equal(1, before.Count);

			_html.RenderCss(true);

			var after = _viewData[getViewDataKey(IncludeType.Css)] as IList<string>;
			Assert.Equal(0, after.Count);
		}

		private static string getViewDataKey(IncludeType type)
		{
			return typeof (IncludeCombinerHtmlExtensions).FullName + "_" + type;
		}
	}
}