using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;

namespace Nrws.Web.IncludeHandling
{
	[DebugFilter]
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
			var model = new IncludeIndexModel { Includes = _combiner.GetAllIncludes(), Combinations = _combiner.GetAllCombinations() };
			return View(model);
		}
	}

	public class IncludeIndexModel
	{
		public IDictionary<string, IncludeCombination> Combinations { get; set; }

		public IEnumerable<Include> Includes { get; set; }
	}
}