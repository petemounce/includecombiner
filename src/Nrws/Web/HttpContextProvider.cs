using System.Web;

namespace Nrws.Web
{
	public class HttpContextProvider : IHttpContextProvider
	{
		private readonly HttpContext _context;

		public HttpContextProvider(HttpContext context)
		{
			_context = context;
		}

		public HttpContextBase Context
		{
			get { return new HttpContextWrapper(_context); }
		}

		public HttpRequestBase Request
		{
			get { return Context.Request; }
		}
	}
}