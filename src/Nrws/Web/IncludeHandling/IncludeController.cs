using System.Text;
using System.Web.Mvc;

namespace Nrws.Web.IncludeHandling
{
	public class IncludeController : Controller
	{
		private readonly IIncludeCombiner _combiner;

		public IncludeController(IIncludeCombiner combiner)
		{
			_combiner = combiner;
		}

		public ActionResult Css(string id)
		{
			var combination = _combiner.GetCombination(id);
			var content = combination == null ? string.Empty : combination.Content;
			return Content(content, MimeTypes.TextCss, Encoding.UTF8);
		}

		public ActionResult Js(string id)
		{
			var combination = _combiner.GetCombination(id);
			var content = combination == null ? string.Empty : combination.Content;
			return Content(content, MimeTypes.ApplicationJavaScript, Encoding.UTF8);
		}

		public ActionResult Index()
		{
			// TODO: list the contents of the IncludeStorage; Includes and IncludeCombinations
			return View();
		}
	}
}