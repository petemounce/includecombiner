using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombinationResult : ActionResult
	{
		private readonly string _key;

		private static readonly IDictionary<IncludeType, string> _contentTypes = new Dictionary<IncludeType, string>
		{
			{ IncludeType.Css, MimeTypes.TextCss }
			, { IncludeType.Js, MimeTypes.ApplicationJavaScript }
		};

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
			_key = key;
			Combination = combiner.GetCombination(_key);
		}

		public IncludeCombinationResult(IncludeCombination combination, string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key");
			}
			_key = key;
			Combination = combination;
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

			var responseBodyBytes = Combination.GetResponseBodyBytes();
			if (responseBodyBytes.Length <= 0)
			{
				context.HttpContext.Response.StatusCode = (int) HttpStatusCode.NoContent;
				return;
			}
			context.HttpContext.Response.AddHeader(HttpHeaders.ContentLength, responseBodyBytes.Length.ToString());
			context.HttpContext.Response.OutputStream.Write(responseBodyBytes, 0, responseBodyBytes.Length);
			context.HttpContext.Response.OutputStream.Flush();
		}
	}
}