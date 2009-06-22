using System.Collections.Generic;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface IIncludeHandlingSettings
	{
		CssTypeElement Css { get; }
		JsTypeElement Js { get; }
		IDictionary<IncludeType, IIncludeTypeSettings> Types { get; }
	}
}