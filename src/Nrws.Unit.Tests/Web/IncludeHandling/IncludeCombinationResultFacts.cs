using System;
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
	public class IncludeCombinationResultFacts
	{
		private readonly MockRepository _mocks;
		private IncludeCombination _cssCombination;
		private readonly HttpContextBase _mockHttpContext;
		private readonly HttpResponseBase _mockResponse;
		private readonly ControllerBase _mockController;
		private readonly ControllerContext _controllerContext;

		public IncludeCombinationResultFacts()
		{
			_mocks = new MockRepository();
			_mockHttpContext = _mocks.StrictMock<HttpContextBase>();
			_mockController = _mocks.StrictMock<ControllerBase>();
			_mockResponse = _mocks.StrictMock<HttpResponseBase>();
			_controllerContext = new ControllerContext(_mockHttpContext, new RouteData(), _mockController);
			_mocks.ReplayAll();
			_cssCombination = new IncludeCombination(IncludeType.Css, new[] { "foo.css" }, "#foo{color:red}", Clock.UtcNow);
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
		public void WhenConstructedViaCombination_CombinationIsSet()
		{
			IncludeCombinationResult result = null;
			Assert.DoesNotThrow(() => result = new IncludeCombinationResult(_cssCombination));
			Assert.Equal(_cssCombination, result.Combination);
		}

		[Fact]
		public void ConstructorThrows_WhenCombinerIsNull()
		{
			Assert.Throws<ArgumentNullException>(() => new IncludeCombinationResult(null, "foo"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void ConstructorThrows_WhenKeyIsBad(string key)
		{
			Assert.Throws<ArgumentException>(() => new IncludeCombinationResult(_mocks.Stub<IIncludeCombiner>(), key));
		}

		[Fact]
		public void WhenNoCombinationExists_ResponseCodeShouldBe404()
		{
			_mockHttpContext.Expect(hc => hc.Response).Return(_mockResponse).Repeat.Twice();
			_mockResponse.Expect(r => r.ContentEncoding = Encoding.UTF8);
			_mockResponse.Expect(r => r.StatusCode = HttpStatusCodes.NotFound);

			var result = new IncludeCombinationResult(null);
			result.ExecuteResult(_controllerContext);

			_mocks.VerifyAll();
		}

		[Fact]
		public void WhenCombinationExists_ShouldCorrectlySetUpResponse()
		{
			_mockHttpContext.Expect(hc => hc.Response).Return(_mockResponse).Repeat.Times(5);
			_mockResponse.Expect(r => r.ContentEncoding = Encoding.UTF8);
			_mockResponse.Expect(r => r.ContentType = MimeTypes.TextCss);
			_mockResponse.Expect(r => r.AddHeader(HttpHeaders.ContentLength, "15"));
			_mockResponse.Expect(r => r.OutputStream).Return(new MemoryStream(8092)).Repeat.Twice();

			var result = new IncludeCombinationResult(_cssCombination);
			result.ExecuteResult(_controllerContext);

			_mocks.VerifyAll();
		}
	}
}