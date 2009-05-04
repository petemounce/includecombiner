using System;
using Nrws.Web.IncludeHandling;
using Xunit;
using Xunit.Extensions;

namespace Nrws.Unit.Tests.Web.IncludeHandling
{
	public class FileSystemIncludeReaderFacts
	{
		private readonly IIncludeReader _reader;

		public FileSystemIncludeReaderFacts()
		{
			_reader = new FileSystemIncludeReader("", "c:\\");
		}

		[Fact]
		public void ToAbsolute_ConvertsPathWhenSourceIsRelative()
		{
			string result = null;
			Assert.DoesNotThrow(() => result = _reader.ToAbsolute("~/foo.css"));
			Assert.Equal("/foo.css", result);
		}

		[Theory]
		[InlineData("")]
		[InlineData(null)]
		public void WhenSourceMissing_WillThrow(string source)
		{
			Assert.Throws<ArgumentException>(() => _reader.Read(source, IncludeType.Css));
		}
	}
}