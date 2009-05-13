using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nrws.Web;
using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeCombinationResultStateFacts
	{
		private readonly ControllerContext _controllerContext;
		private readonly IncludeCombination _cssCombination;
		private readonly MockRepository _mocks;
		private readonly HttpCachePolicyBase _stubCache;
		private readonly IIncludeCombiner _stubCombiner;
		private readonly ControllerBase _stubController;
		private readonly HttpContextBase _stubHttpContext;
		private readonly HttpResponseBase _stubResponse;
		private readonly HttpRequestBase _stubRequest;

		public IncludeCombinationResultStateFacts()
		{
			_mocks = new MockRepository();
			_stubHttpContext = _mocks.Stub<HttpContextBase>();
			_stubController = _mocks.Stub<ControllerBase>();
			_stubRequest = _mocks.Stub<HttpRequestBase>();
			_stubResponse = _mocks.Stub<HttpResponseBase>();
			_stubCache = _mocks.Stub<HttpCachePolicyBase>();
			_controllerContext = new ControllerContext(_stubHttpContext, new RouteData(), _stubController);
			_stubCombiner = _mocks.Stub<IIncludeCombiner>();

			_mocks.ReplayAll();
			_cssCombination = new IncludeCombination(IncludeType.Css, new[] { "foo.css" }, "#foo{color:red;}", Clock.UtcNow);
		}

		[Fact]
		public void WhenConstructedViaCombinerAndKey_CombinationIsSet()
		{
			var mockCombiner = _mocks.StrictMock<IIncludeCombiner>();
			_mocks.ReplayAll();
			mockCombiner.Expect(c => c.GetCombination("foo")).Return(_cssCombination);
			IncludeCombinationResult result = null;
			Assert.DoesNotThrow(() => result = new IncludeCombinationResult(mockCombiner, "foo"));
			Assert.Equal(_cssCombination, result.Combination);
		}

		[Fact]
		public void ConstructorThrows_WhenCombinerIsNull()
		{
			const IIncludeCombiner combiner = null;
			Assert.Throws<ArgumentNullException>(() => new IncludeCombinationResult(combiner, "foo"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void ConstructorThrows_WhenKeyIsBad(string key)
		{
			Assert.Throws<ArgumentException>(() => new IncludeCombinationResult(_mocks.Stub<IIncludeCombiner>(), key));
		}

		[Fact]
		public void WhenCombinationContainsNoContent_ShouldNotThrow()
		{
			_stubHttpContext.Expect(hc => hc.Response).Return(_stubResponse);
			_stubHttpContext.Expect(hc => hc.Request).Return(_stubRequest);
			_stubRequest.Expect(r => r.Headers).Return(new NameValueCollection { {HttpHeaders.AcceptEncoding, "" } });
			_stubResponse.ContentEncoding = Encoding.UTF8;
			_stubResponse.ContentType = MimeTypes.TextCss;
			_stubResponse.AddHeader(HttpHeaders.ContentLength, "15");
			_stubResponse.Expect(r => r.OutputStream).Return(new MemoryStream(8092)).Repeat.Twice();
			_stubResponse.Expect(r => r.Cache).Return(_stubCache);

			var emptyCombination = new IncludeCombination(IncludeType.Css, new[] { "foo.css" }, "", Clock.UtcNow);
			_stubCombiner.Expect(c => c.GetCombination("foo")).Return(emptyCombination);
			var result = new IncludeCombinationResult(_stubCombiner, "foo");
			Assert.DoesNotThrow(() => result.ExecuteResult(_controllerContext));
		}
	}
}