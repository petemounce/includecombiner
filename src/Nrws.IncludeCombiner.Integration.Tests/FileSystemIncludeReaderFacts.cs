using System;

using Xunit;
using Xunit.Extensions;

namespace Nrws.IncludeCombiner.Integration.Tests
{
	public class FileSystemIncludeReaderFacts
	{
		private readonly IIncludeReader reader;

		public FileSystemIncludeReaderFacts()
		{
			reader = new FileSystemIncludeReader();
		}

		[Fact]
		public void WhenFileExists_WillReadIt()
		{
			string content = null;
			Assert.DoesNotThrow(() => content = reader.Read("tests\\exists.txt"));
			Assert.Equal("hello world", content);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void WhenSourceMissing_WillThrow(string source)
		{
			Assert.Throws<ArgumentException>(() => reader.Read(source));
		}

		[Fact]
		public void WhenFileNotFound_WillThrow()
		{
			Assert.Throws<InvalidOperationException>(() => reader.Read("c:\\doesnotexist.txt"));
		}
	}
}