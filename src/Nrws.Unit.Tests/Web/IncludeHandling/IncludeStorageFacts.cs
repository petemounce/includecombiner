using System;
using Nrws.Web.IncludeHandling;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class IncludeStorageFacts
	{
		private readonly IIncludeStorage _storage;

		public IncludeStorageFacts()
		{
			_storage = new StaticIncludeStorage();
		}

		[Fact]
		public void StoreInclude_ShouldThrowWhenNull()
		{
			const Include include = null;
			Assert.Throws<ArgumentNullException>(() => _storage.Store(include));
		}

		[Fact]
		public void StoreInclude_DoesNotThrow_WhenIncludeIsValid()
		{
			var include = new Include(IncludeType.Js, "/foo.js", "alert('foo');", Clock.UtcNow);
			Assert.DoesNotThrow(() => _storage.Store(include));
		}

		[Fact]
		public void StoreIncludeTwice_DoesNotThrow()
		{
			var include = new Include(IncludeType.Js, "/foo.js", "alert('foo');", Clock.UtcNow);
			Assert.DoesNotThrow(() => _storage.Store(include));
			Assert.DoesNotThrow(() => _storage.Store(include));
		}

		[Fact]
		public void StoreCombination_ShouldThrowWhenNull()
		{
			const IncludeCombination combination = null;
			Assert.Throws<ArgumentNullException>(() => _storage.Store(combination));
		}

		[Fact]
		public void StoreCombination_DoesNotThrow_WhenCombinationIsValid()
		{
			var combination = new IncludeCombination("foo", IncludeType.Css, "#foo {color:red}", Clock.UtcNow);
			Assert.DoesNotThrow(() => _storage.Store(combination));
		}

		[Fact]
		public void StoreCombinationTwice_DoesNotThrow()
		{
			var combination = new IncludeCombination("foo", IncludeType.Css, "#foo {color:red}", Clock.UtcNow);
			Assert.DoesNotThrow(() => _storage.Store(combination));
			Assert.DoesNotThrow(() => _storage.Store(combination));
		}

		[Fact]
		public void GetCombination_WhenCombinationExists_DoesNotThrow()
		{
			var combination = new IncludeCombination("foo", IncludeType.Css, "#foo {color:red}", Clock.UtcNow);
			_storage.Store(combination);
			IncludeCombination result = null;
			Assert.DoesNotThrow(() => result = _storage.GetCombination("foo"));

			Assert.Equal(combination.Content, result.Content);
		}

		[Fact]
		public void GetCombination_WhenCombinationDoesNotExist_ReturnsNull()
		{
			IncludeCombination result = null;
			Assert.DoesNotThrow(() => result = _storage.GetCombination("flsihjdf"));
			Assert.Null(result);
		}
	}
}