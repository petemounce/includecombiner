using System;
using System.Text;
using System.Web.Mvc;
using Nrws.Web;
using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeControllerFacts
	{
		private readonly MockRepository _mocks;
		private readonly IIncludeCombiner _mockCombiner;
		private readonly IncludeController _controller;
		private readonly IncludeCombination _cssCombination;
		private readonly IncludeCombination _jsCombination;

		public IncludeControllerFacts()
		{
			_mocks = new MockRepository();
			_mockCombiner = _mocks.StrictMock<IIncludeCombiner>();
			_controller = new IncludeController(_mockCombiner);
			_mocks.ReplayAll();
			_cssCombination = new IncludeCombination(IncludeType.Css, new [] {"~/content/css/foo.css"}, "#foo {color:red;}", DateTime.UtcNow);
			_jsCombination = new IncludeCombination(IncludeType.Js, new[] { "~/content/js/foo.js" }, "alert('foo');", DateTime.UtcNow);
		}

		[Fact]
		public void Css_ShouldAskCombinerForCombinationMatchingKey()
		{
			_mockCombiner.Expect(c => c.GetCombination("foo")).Return(_cssCombination);
			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Css("foo"));

			Assert.IsType<ContentResult>(result);
			var content = (ContentResult) result;
			Assert.Equal(_cssCombination.Content, content.Content);
			Assert.Equal(MimeTypes.TextCss, content.ContentType);
			Assert.Equal(Encoding.UTF8, content.ContentEncoding);
			_mocks.VerifyAll();
		}

		[Fact]
		public void Js_ShouldAskCombinerForCombinationMatchingKey()
		{
			_mockCombiner.Expect(c => c.GetCombination("foo")).Return(_jsCombination);
			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Js("foo"));

			Assert.IsType<ContentResult>(result);
			var content = (ContentResult)result;
			Assert.Equal(_jsCombination.Content, content.Content);
			Assert.Equal(MimeTypes.ApplicationJavaScript, content.ContentType);
			Assert.Equal(Encoding.UTF8, content.ContentEncoding);
			_mocks.VerifyAll();
		}

		[Fact]
		public void Css_ShouldRenderBlank_WhenNoCombinationMatchesKey()
		{
			_mockCombiner.Expect(c => c.GetCombination("foo")).Return(null);
			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Css("foo"));

			Assert.IsType<ContentResult>(result);
			var content = (ContentResult)result;
			Assert.Equal(string.Empty, content.Content);
			Assert.Equal(MimeTypes.TextCss, content.ContentType);
			Assert.Equal(Encoding.UTF8, content.ContentEncoding);
			_mocks.VerifyAll();
		}

		[Fact]
		public void Js_ShouldRenderBlank_WhenNoCombinationMatchesKey()
		{
			_mockCombiner.Expect(c => c.GetCombination("foo")).Return(null);
			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Js("foo"));

			Assert.IsType<ContentResult>(result);
			var content = (ContentResult)result;
			Assert.Equal(string.Empty, content.Content);
			Assert.Equal(MimeTypes.ApplicationJavaScript, content.ContentType);
			Assert.Equal(Encoding.UTF8, content.ContentEncoding);
			_mocks.VerifyAll();
		}
	}
}