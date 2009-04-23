using System;
using Nrws.Web.IncludeHandling;
using Xunit;

namespace Nrws.Integration.Tests.Web.IncludeHandling
{
	public class FileSystemIncludeReaderFacts
	{
		private readonly IIncludeReader _reader;

		public FileSystemIncludeReaderFacts()
		{
			_reader = new FileSystemIncludeReader("/", Environment.CurrentDirectory);
		}

		[Fact]
		public void WhenFileExists_WillReadIt()
		{
			Include include = null;
			Assert.DoesNotThrow(() => include = _reader.Read("tests\\exists.txt", IncludeType.Js));
			Assert.Equal("hello world", include.Content);
		}

		[Fact]
		public void WhenFileNotFound_WillThrow()
		{
			Assert.Throws<InvalidOperationException>(() => _reader.Read("c:\\doesNotExist.txt", IncludeType.Css));
		}
	}
}