using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Nrws.Web.IncludeHandling.Configuration;

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

		private static readonly IDictionary<ResponseCompression, Func<string, bool>> _compressionOrder = new Dictionary<ResponseCompression, Func<string, bool>>
		{
			{ ResponseCompression.Gzip, header => header.Contains("gzip") },
			{ ResponseCompression.Deflate, header => header.Contains("deflate") },
			{ ResponseCompression.None, header => true }
		};

		private readonly string _key;
		private DateTime _now;
		private TimeSpan? _cacheFor;
		private readonly IList<ResponseCompression> _preferredCompressionOrder;

		public IncludeCombination Combination { get; private set; }

		public IncludeCombinationResult(IIncludeCombiner combiner, string key, DateTime now)
		{
			if (combiner == null)
			{
				throw new ArgumentNullException("combiner");
			}
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key");
			}
			_key = key;
			_now = now;
			Combination = combiner.GetCombination(_key);
		}

		public IncludeCombinationResult(IIncludeCombiner combiner, string key, DateTime now, IIncludeHandlingSettings settings)
			: this(combiner, key, now)
		{
			var typeSettings = settings.Types[Combination.Type];
			_cacheFor = typeSettings.CacheFor;
			_preferredCompressionOrder = typeSettings.CompressionOrder;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
			if (Combination == null || Combination.Content == null)
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NotFound;
				return;
			}
			context.HttpContext.Response.ContentType = _contentTypes[Combination.Type];
			if (_cacheFor.HasValue)
			{
				context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
				context.HttpContext.Response.Cache.SetExpires(_now.Add(_cacheFor.Value));
				context.HttpContext.Response.Cache.SetMaxAge(_cacheFor.Value);
				context.HttpContext.Response.Cache.SetValidUntilExpires(true);
				context.HttpContext.Response.Cache.SetLastModified(Combination.LastModifiedAt);
			}
			context.HttpContext.Response.Cache.SetETag(_key + "-" + Combination.LastModifiedAt.Ticks);
			var compressionAccepted = figureOutCompression(context.HttpContext.Request);
			var responseBodyBytes = Combination.Bytes[compressionAccepted];

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

		private ResponseCompression figureOutCompression(HttpRequestBase request)
		{
			var acceptEncoding = request.Headers[HttpHeaders.AcceptEncoding];
			if (string.IsNullOrEmpty(acceptEncoding) || isIe6OrLess(request.Browser))
			{
				return ResponseCompression.None;
			}
			acceptEncoding = acceptEncoding.Trim().ToLowerInvariant();

			foreach(var compression in _preferredCompressionOrder)
			{
				if (_compressionOrder[compression](acceptEncoding))
				{
					return compression;
				}
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