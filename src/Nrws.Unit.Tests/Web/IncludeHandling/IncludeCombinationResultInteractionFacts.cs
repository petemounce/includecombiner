using System.IO;
using System.Net;
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
	public class IncludeCombinationResultInteractionFacts
	{
		private readonly ControllerContext _controllerContext;
		private readonly IncludeCombination _cssCombination;
		private readonly HttpCachePolicyBase _mockCachePolicy;
		private readonly ControllerBase _mockController;
		private readonly HttpContextBase _mockHttpContext;
		private readonly HttpResponseBase _mockResponse;
		private readonly MockRepository _mocks;
		private readonly IIncludeCombiner _stubCombiner;
		private readonly HttpRequestBase _mockRequest;

		public IncludeCombinationResultInteractionFacts()
		{
			_mocks = new MockRepository();
			_mockHttpContext = _mocks.StrictMock<HttpContextBase>();
			_mockController = _mocks.StrictMock<ControllerBase>();
			_mockResponse = _mocks.StrictMock<HttpResponseBase>();
			_mockRequest = _mocks.StrictMock<HttpRequestBase>();
			_mockCachePolicy = _mocks.StrictMock<HttpCachePolicyBase>();
			_controllerContext = new ControllerContext(_mockHttpContext, new RouteData(), _mockController);
			_stubCombiner = _mocks.Stub<IIncludeCombiner>();
			_mocks.ReplayAll();
			_cssCombination = new IncludeCombination(IncludeType.Css, new[] { "foo.css" }, "#foo{color:red}", Clock.UtcNow);
		}

		[Fact]
		public void WhenNoCombinationExists_ResponseCodeShouldBe404()
		{
			_mockHttpContext.Expect(hc => hc.Response).Return(_mockResponse).Repeat.Twice();
			_mockResponse.Expect(r => r.ContentEncoding = Encoding.UTF8);
			_mockResponse.Expect(r => r.StatusCode = (int) HttpStatusCode.NotFound);
			_stubCombiner.Expect(c => c.GetCombination("foo")).Return(null);

			var result = new IncludeCombinationResult(_stubCombiner, "foo");
			Assert.DoesNotThrow(() => result.ExecuteResult(_controllerContext));

			_mocks.VerifyAll();
		}

		[Fact]
		public void WhenCombinationExists_ShouldCorrectlySetUpResponse()
		{
			_mockHttpContext.Expect(hc => hc.Response).Return(_mockResponse).Repeat.Times(6);
			_mockHttpContext.Expect(hc => hc.Request).Return(_mockRequest);
			_mockRequest.Expect(r => r.Headers[HttpHeaders.AcceptEncoding]).Return("");
			_mockResponse.Expect(r => r.ContentEncoding = Encoding.UTF8);
			_mockResponse.Expect(r => r.ContentType = MimeTypes.TextCss);
			_mockResponse.Expect(r => r.AddHeader(HttpHeaders.ContentLength, "15"));
			_mockResponse.Expect(r => r.OutputStream).Return(new MemoryStream(8092)).Repeat.Twice();
			_mockResponse.Expect(r => r.Cache).Return(_mockCachePolicy);
			_mockCachePolicy.Expect(cp => cp.SetETag(Arg<string>.Matches(etag => etag.StartsWith("foo") && etag.EndsWith(_cssCombination.LastModifiedAt.Ticks.ToString()))));
			_stubCombiner.Expect(c => c.GetCombination("foo")).Return(_cssCombination);

			var result = new IncludeCombinationResult(_stubCombiner, "foo");
			Assert.DoesNotThrow(() => result.ExecuteResult(_controllerContext));

			_mocks.VerifyAll();
		}
	}
}