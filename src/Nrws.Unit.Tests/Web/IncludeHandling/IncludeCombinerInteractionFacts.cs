using System.Collections.Generic;
using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeCombinerInteractionFacts
	{
		private readonly IIncludeCombiner _combiner;
		private readonly IIncludeReader _mockReader;
		private readonly MockRepository _mocks;
		private readonly IIncludeStorage _mockStorage;

		public IncludeCombinerInteractionFacts()
		{
			_mocks = new MockRepository();
			_mockReader = _mocks.StrictMock<IIncludeReader>();
			_mockStorage = _mocks.StrictMock<IIncludeStorage>();
			_combiner = new IncludeCombiner(_mockReader, _mockStorage);
			_mocks.ReplayAll();
		}

		[Fact]
		public void RegisterInclude_ShouldAskStorageToStoreIt()
		{
			var include = new Include(IncludeType.Js, "foo.js", "alert('');", Clock.UtcNow);
			_mockReader.Expect(r => r.Read("~/foo.js", include.Type)).Return(include);
			_mockStorage.Expect(s => s.Store(include));
			Assert.DoesNotThrow(() => _combiner.RegisterInclude("~/foo.js", IncludeType.Js));
			_mocks.VerifyAll();
		}

		[Fact]
		public void GetCombination_ShouldAskStorageForCombination()
		{
			_mockStorage.Expect(s => s.GetCombination("foo")).Return(new IncludeCombination(IncludeType.Css, new[] { "~/content/css/foo.css" }, ".foo{}", Clock.UtcNow));
			IncludeCombination combination = null;
			Assert.DoesNotThrow(() => combination = _combiner.GetCombination("foo"));
			Assert.NotNull(combination);
			_mocks.VerifyAll();
		}

		[Fact]
		public void GetAllIncludes_ShouldAskStorageForIncludes()
		{
			_mockStorage.Expect(s => s.GetAllIncludes()).Return(new[] { new Include(IncludeType.Css, "~/foo.css", "#foo{color:red;}", Clock.UtcNow) });
			IEnumerable<Include> includes = null;
			Assert.DoesNotThrow(() => includes = _combiner.GetAllIncludes());
			Assert.NotNull(includes);
			_mocks.VerifyAll();
		}

		[Fact]
		public void GetAllCombinations_ShouldAskStorageForCombinations()
		{
			_mockStorage.Expect(s => s.GetAllCombinations()).Return(new Dictionary<string, IncludeCombination>());
			IDictionary<string, IncludeCombination> combinations = null;
			Assert.DoesNotThrow(() => combinations = _combiner.GetAllCombinations());
			Assert.NotNull(combinations);
			_mocks.VerifyAll();
		}
	}
}