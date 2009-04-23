using System.Collections.Specialized;
using System.Web;

namespace Nrws.Web
{
	public interface IHttpContextProvider
	{
		HttpContextBase Context { get; }
		HttpRequestBase Request { get; }
		HttpResponseBase Response { get; }
		NameValueCollection FormOrQueryString { get; }
	}
}