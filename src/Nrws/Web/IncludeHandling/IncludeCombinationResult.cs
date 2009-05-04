using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Mvc;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeCombinationResult : ActionResult
	{
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
			Combination = combiner.GetCombination(key);
		}

		public IncludeCombinationResult(IncludeCombination combination)
		{
			Combination = combination;
		}

		public IncludeCombination Combination { get; private set; }

		public override void ExecuteResult(ControllerContext context)
		{
			context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
			if (Combination == null || Combination.Content == null)
			{
				context.HttpContext.Response.StatusCode = HttpStatusCodes.NotFound;
				return;
			}
			context.HttpContext.Response.ContentType = _contentTypes[Combination.Type];
			byte[] responseBodyBytes;
			using (var memoryStream = new MemoryStream(8092))
			{
				var noCompression = Encoding.UTF8.GetBytes(Combination.Content);
				memoryStream.Write(noCompression, 0, noCompression.Length);
				responseBodyBytes = memoryStream.ToArray();
			}
			context.HttpContext.Response.AddHeader(HttpHeaders.ContentLength, responseBodyBytes.Length.ToString());
			context.HttpContext.Response.OutputStream.Write(responseBodyBytes, 0, responseBodyBytes.Length);
			context.HttpContext.Response.OutputStream.Flush();
		}
	}
}