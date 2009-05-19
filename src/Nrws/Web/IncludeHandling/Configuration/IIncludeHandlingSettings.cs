namespace Nrws.Web.IncludeHandling.Configuration
{
	public interface IIncludeHandlingSettings
	{
		bool AllowDebug { get; }
		CssElement Css { get; }
		JsElement Js { get; }
	}
}