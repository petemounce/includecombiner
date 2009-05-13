using System;
using System.Collections.Generic;
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

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Css(string id)
		{
			return new IncludeCombinationResult(_combiner, id, DateTime.UtcNow, TimeSpan.FromDays(365));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Js(string id)
		{
			return new IncludeCombinationResult(_combiner, id, DateTime.UtcNow, TimeSpan.FromDays(365));
		}

		[AcceptVerbs(HttpVerbs.Get)]
		public ActionResult Index()
		{
			// TODO: list the contents of the IncludeStorage; Includes and IncludeCombinations
			var model = new IncludeIndexModel { Includes = _combiner.GetAllIncludes(), Combinations = _combiner.GetAllCombinations() };
			return View(model);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Clear()
		{
			_combiner.Clear();
			return RedirectToAction("index");
		}
	}

	public class IncludeIndexModel
	{
		public IDictionary<string, IncludeCombination> Combinations { get; set; }

		public IEnumerable<Include> Includes { get; set; }
	}
}