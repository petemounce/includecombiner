using System.Configuration;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public class JsTypeElement : IncludeTypeElement, IJsMinifySettings
	{
		[ConfigurationProperty(OPTIONS)]
		private JsOptionsElement jsOptions
		{
			get { return (JsOptionsElement) this[OPTIONS] ?? new JsOptionsElement(); }
		}

		#region IJsMinifySettings Members

		public bool Verbose
		{
			get { return jsOptions.Verbose; }
		}

		public bool Obfuscate
		{
			get { return jsOptions.Obfuscate; }
		}

		public bool PreserveSemiColons
		{
			get { return jsOptions.PreserveSemiColons; }
		}

		public bool DisableOptimizations
		{
			get { return jsOptions.DisableOptimizations; }
		}

		#endregion
	}
}