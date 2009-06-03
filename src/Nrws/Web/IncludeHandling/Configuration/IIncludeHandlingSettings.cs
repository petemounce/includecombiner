using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface IIncludeHandlingSettings
	{
		CssElement Css { get; }
		JsElement Js { get; }
		IDictionary<IncludeType, IncludeTypeElement> Types { get; }
	}
}