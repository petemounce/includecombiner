using System.Configuration;

namespace Nrws.Web.IncludeHandling.Configuration
{
	public class JsOptionsElement : ConfigurationElement, IJsMinifySettings
	{
		private const string DISABLEOPTIMIZATIONS = "disableOptimizations";
		private const string OBFUSCATE = "obfuscate";
		private const string PRESERVESEMICOLONS = "preserveSemiColons";
		private const string VERBOSE = "verbose";

		#region IJsMinifySettings Members

		[ConfigurationProperty(VERBOSE, DefaultValue = false)]
		public bool Verbose
		{
			get { return (bool) this[VERBOSE]; }
		}

		[ConfigurationProperty(OBFUSCATE, DefaultValue = true)]
		public bool Obfuscate
		{
			get { return (bool) this[OBFUSCATE]; }
		}

		[ConfigurationProperty(PRESERVESEMICOLONS, DefaultValue = true)]
		public bool PreserveSemiColons
		{
			get { return (bool) this[PRESERVESEMICOLONS]; }
		}

		[ConfigurationProperty(DISABLEOPTIMIZATIONS, DefaultValue = false)]
		public bool DisableOptimizations
		{
			get { return (bool) this[DISABLEOPTIMIZATIONS]; }
		}

		#endregion
	}
}