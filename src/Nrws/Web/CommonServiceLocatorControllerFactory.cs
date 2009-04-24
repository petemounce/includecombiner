using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;

namespace Nrws.Web
{
	public class CommonServiceLocatorControllerFactory : DefaultControllerFactory
	{
		protected override IController GetControllerInstance(Type controllerType)
		{
			return (controllerType == null) ? base.GetControllerInstance(controllerType) : ServiceLocator.Current.GetInstance(controllerType) as IController;
		}
	}
}