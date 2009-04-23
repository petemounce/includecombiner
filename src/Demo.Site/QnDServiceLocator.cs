using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Nrws;
using Nrws.Web;
using Nrws.Web.IncludeHandling;

namespace Demo.Site
{
	public class QnDServiceLocator : IServiceLocator
	{
		private static IDictionary<Type, object> _types;

		public QnDServiceLocator(IDictionary<Type, object> types)
		{
			_types = types;
		}

		public object GetService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		public object GetInstance(Type serviceType)
		{
			return _types[serviceType];
		}

		public object GetInstance(Type serviceType, string key)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<object> GetAllInstances(Type serviceType)
		{
			throw new NotImplementedException();
		}

		public TService GetInstance<TService>()
		{
			return (TService) GetInstance(typeof (TService));
		}

		public TService GetInstance<TService>(string key)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			throw new NotImplementedException();
		}

		public static QnDServiceLocator Create(IHttpContextProvider http)
		{
			var types = new Dictionary<Type, object>
			{
				{ typeof (IHttpContextProvider), http },
				{ typeof (IKeyGenerator), new KeyGenerator() },
				{ typeof (IIncludeStorage), new StaticIncludeStorage() }
			};
			types.Add(typeof (IIncludeReader), new FileSystemIncludeReader((IHttpContextProvider) types[typeof (IHttpContextProvider)]));

			var includeReader = (IIncludeReader) types[typeof (IIncludeReader)];
			var keyGen = (IKeyGenerator) types[typeof (IKeyGenerator)];
			var storage = (IIncludeStorage) types[typeof (IIncludeStorage)];
			var combiner = new IncludeCombiner(includeReader, keyGen, storage);
			types.Add(typeof (IIncludeCombiner), combiner);
			return new QnDServiceLocator(types);
		}
	}
}