using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Nrws.Web.IncludeHandling
{
	public enum ResponseCompression
	{
		None = 0,
		Gzip = 1,
		Deflate = 2
	}

	public class IncludeCombinationResult : ActionResult
	{
		private static readonly IDictionary<IncludeType, string> _contentTypes = new Dictionary<IncludeType, string>
		{
			{ IncludeType.Css, MimeTypes.TextCss }
			, { IncludeType.Js, MimeTypes.ApplicationJavaScript }
		};

		private readonly IIncludeCombiner _combiner;
		private readonly string _key;

		public IncludeCombinationResult(IIncludeCombiner combiner, string key)
		{
			if (combiner == null)
			{
				throw new ArgumentNullException("combiner");
			}
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key");
			}
			_combiner = combiner;
			_key = key;
			Combination = combiner.GetCombination(_key);
		}

		public IncludeCombination Combination { get; private set; }

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
			if (Combination == null || Combination.Content == null)
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
				return;
			}
			context.HttpContext.Response.ContentType = _contentTypes[Combination.Type];
			context.HttpContext.Response.Cache.SetETag(_key + "-" + Combination.LastModifiedAt.Ticks);
			var compressionAccepted = figureOutCompression(context.HttpContext.Request);
			var responseBodyBytes = Combination.GetResponseBodyBytes(compressionAccepted);
			_combiner.UpdateCombination(Combination);
			if (responseBodyBytes.Length <= 0)
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NoContent;
				return;
			}
			switch (compressionAccepted)
			{
				case ResponseCompression.None:
					break;
				case ResponseCompression.Gzip:
				case ResponseCompression.Deflate:
					context.HttpContext.Response.AppendHeader(HttpHeaders.ContentEncoding, compressionAccepted.ToString().ToLowerInvariant());
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			context.HttpContext.Response.AddHeader(HttpHeaders.ContentLength, responseBodyBytes.Length.ToString());
			context.HttpContext.Response.OutputStream.Write(responseBodyBytes, 0, responseBodyBytes.Length);
			context.HttpContext.Response.OutputStream.Flush();
		}

		private static ResponseCompression figureOutCompression(HttpRequestBase request)
		{
			var acceptEncoding = request.Headers[HttpHeaders.AcceptEncoding];
			if (string.IsNullOrEmpty(acceptEncoding) || isIe6OrLess(request.Browser))
			{
				return ResponseCompression.None;
			}
			acceptEncoding = acceptEncoding.Trim().ToLowerInvariant();
			// NOTE: Firefox will send "gzip,deflate".  Search for "gzip" first because it compresses smaller - though deflate uses less CPU.
			// TODO: Make this preference configurable...?
			if (acceptEncoding.Contains("gzip"))
			{
				return ResponseCompression.Gzip;
			}
			if (acceptEncoding.Contains("deflate"))
			{
				return ResponseCompression.Deflate;
			}
			return ResponseCompression.None;
		}

		private static bool isIe6OrLess(HttpBrowserCapabilitiesBase browser)
		{
			return
				browser.Type.Contains("IE") && browser.MajorVersion <= 6;
		}
	}
}