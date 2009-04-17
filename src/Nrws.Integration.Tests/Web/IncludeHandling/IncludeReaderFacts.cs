using System;

using Nrws.Web.IncludeHandling;

using Xunit;
using Xunit.Extensions;

namespace Nrws.Integration.Tests.Web.IncludeHandling
{
	public class IncludeReaderFacts
	{
		private readonly IIncludeReader _reader;

		public IncludeReaderFacts()
		{
			_reader = new IncludeReader();
		}

		[Fact]
		public void WhenFileExists_WillReadIt()
		{
			Include include = null;
			Assert.DoesNotThrow(() => include = _reader.Read("tests\\exists.txt", IncludeType.Script));
			Assert.Equal("hello world", include.Content);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void WhenSourceMissing_WillThrow(string source)
		{
			Assert.Throws<ArgumentException>(() => _reader.Read(source, IncludeType.Css));
		}

		[Fact]
		public void WhenFileNotFound_WillThrow()
		{
			Assert.Throws<InvalidOperationException>(() => _reader.Read("c:\\doesNotExist.txt", IncludeType.Css));
		}
	}
}