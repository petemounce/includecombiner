using System;
using System.Collections.Generic;
using System.Web.Mvc;
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

		public IncludeControllerFacts()
		{
			_mocks = new MockRepository();
			_mockCombiner = _mocks.StrictMock<IIncludeCombiner>();
			_controller = new IncludeController(_mockCombiner);
			_mocks.ReplayAll();
		}

		[Fact]
		public void Css_ShouldAskCombinerForCombinationMatchingKey()
		{
			var combination = new IncludeCombination(IncludeType.Css, new[] { "foo.css" }, "#Foo{color:red;}", DateTime.UtcNow);
			_mockCombiner.Expect(c => c.GetCombination("foo")).Return(combination);
			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Css("foo"));

			Assert.IsType<IncludeCombinationResult>(result);
			Assert.Equal(combination, ((IncludeCombinationResult)result).Combination);
			_mocks.VerifyAll();
		}

		[Fact]
		public void Js_ShouldAskCombinerForCombinationMatchingKey()
		{
			var combination = new IncludeCombination(IncludeType.Js, new[] { "foo.js" }, "alert('foo!');", DateTime.UtcNow);
			_mockCombiner.Expect(c => c.GetCombination("foo")).Return(combination);
			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Js("foo"));

			Assert.IsType<IncludeCombinationResult>(result);
			Assert.Equal(combination, ((IncludeCombinationResult)result).Combination);
			_mocks.VerifyAll();
		}

		[Fact]
		public void Index_ShouldAskCombinerForAllCombinations_AndAllIncludes()
		{
			_mockCombiner.Expect(c => c.GetAllIncludes()).Return(new List<Include>());
			_mockCombiner.Expect(c => c.GetAllCombinations()).Return(new Dictionary<string, IncludeCombination>());

			ActionResult result = null;
			Assert.DoesNotThrow(() => result = _controller.Index());
			Assert.IsType<ViewResult>(result);
			_mocks.VerifyAll();
		}
	}
}