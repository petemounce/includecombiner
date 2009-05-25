using System.Configuration;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public class IncludeHandlingSectionHandler : ConfigurationSection, IIncludeHandlingSettings
	{
		private const string ALLOWDEBUG = "allowDebug";
		private const string CSS = "css";
		private const string JS = "js";

		#region IIncludeHandlingSettings Members

		[ConfigurationProperty(ALLOWDEBUG)]
		public bool AllowDebug
		{
			get { return (bool) this[ALLOWDEBUG]; }
		}

		[ConfigurationProperty(CSS)]
		public CssElement Css
		{
			get { return (CssElement) this[CSS] ?? new CssElement(); }
		}

		[ConfigurationProperty(JS)]
		public JsElement Js
		{
			get { return (JsElement) this[JS] ?? new JsElement(); }
		}

		#endregion
	}
}