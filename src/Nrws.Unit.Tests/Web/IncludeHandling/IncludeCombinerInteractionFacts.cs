using Nrws.Web.IncludeHandling;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeCombinerInteractionFacts
	{
		private readonly IKeyGenerator _mockKeyGen;
		private readonly IIncludeReader _mockReader;
		private readonly IIncludeStorage _mockStorage;
		private readonly IIncludeCombiner _combiner;
		private readonly MockRepository _mocks;

		public IncludeCombinerInteractionFacts()
		{
			_mocks = new MockRepository();
			_mockKeyGen = _mocks.StrictMock<IKeyGenerator>();
			_mockReader = _mocks.StrictMock<IIncludeReader>();
			_mockStorage = _mocks.StrictMock<IIncludeStorage>();
			_combiner = new IncludeCombiner(_mockReader, _mockKeyGen, _mockStorage);
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
	}
}