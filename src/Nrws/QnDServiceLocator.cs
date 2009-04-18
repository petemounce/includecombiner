using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Nrws.Web.IncludeHandling;

namespace Nrws
{
	public class QnDServiceLocator : IServiceLocator
	{
		private static readonly IDictionary<Type, object> _types;

		static QnDServiceLocator()
		{
			_types = new Dictionary<Type, object>
			{
				{ typeof (IIncludeReader), new IncludeReader() },
				{ typeof(IKeyGenerator), new KeyGenerator()},
				{ typeof(IIncludeStorage), new MemoryIncludeStorage()}
			};
			var includeReader = (IIncludeReader) _types[typeof (IIncludeReader)];
			var keyGen = (IKeyGenerator)_types[typeof(IKeyGenerator)];
			var storage = (IIncludeStorage) _types[typeof (IIncludeStorage)];
			var combiner = new IncludeCombiner(includeReader, keyGen, storage);
			_types.Add(typeof (IIncludeCombiner), combiner);
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
	}
}