using System;
using System.Collections.Generic;

using Microsoft.Practices.ServiceLocation;

namespace Nrws.IncludeCombiner
{
	public class QnDServiceLocator : IServiceLocator
	{
		private readonly IDictionary<Type, object> _types;

		public QnDServiceLocator()
		{
			_types = new Dictionary<Type, object>
			{
				{ typeof (IIncludeCombiner), new IncludeCombiner() }
			};
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
			return (TService)GetInstance(typeof (TService));
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