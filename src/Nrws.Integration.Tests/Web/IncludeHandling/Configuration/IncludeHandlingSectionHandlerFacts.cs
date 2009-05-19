using System;
using System.Collections.Generic;
using System.Configuration;
using Nrws.Web.IncludeHandling;
using Nrws.Web.IncludeHandling.Configuration;
using Xunit;
using Yahoo.Yui.Compressor;

namespace Nrws.Integration.Tests.Web.IncludeHandling.Configuration
{
	public class IncludeHandlingSectionHandlerFacts
	{
		[Fact]
		public void DefaultsAreCorrect()
		{
			var section = (IIncludeHandlingSettings) ConfigurationManager.GetSection("includeHandling");
			Assert.False(section.AllowDebug);

			Assert.Equal("~/include/{0}/{1}", section.Css.Path);
			Assert.Equal(TimeSpan.FromDays(365), section.Css.CacheFor);
			var expected = new List<ResponseCompression> { ResponseCompression.Gzip, ResponseCompression.Deflate };
			Assert.Equal(expected[0], section.Css.CompressionOrder[0]);
			Assert.Equal(expected[1], section.Css.CompressionOrder[1]);
			Assert.Equal(int.MaxValue, section.Css.LineBreakAt);
			Assert.Equal(true, section.Css.Minify);

			Assert.Equal("~/include/{0}/{1}", section.Js.Path);
			Assert.Equal(TimeSpan.FromDays(365), section.Js.CacheFor);
			Assert.Equal(expected[0], section.Js.CompressionOrder[0]);
			Assert.Equal(expected[1], section.Js.CompressionOrder[1]);
			Assert.Equal(int.MaxValue, section.Js.LineBreakAt);
			Assert.Equal(true, section.Js.Minify);

			Assert.Equal(CssCompressionType.StockYuiCompressor, section.Css.CompressionType);

			Assert.Equal(false, section.Js.DisableOptimizations);
			Assert.Equal(true, section.Js.Obfuscate);
			Assert.Equal(true, section.Js.PreserveSemiColons);
			Assert.Equal(false, section.Js.Verbose);
		}
	}
}