using Autofac;
using Service.Backoffice.Services;

namespace Service.Backoffice.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<KeyValueDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<EducationProgressDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<RetryDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<UserTokenAccountDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<MarketProductDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<UserResolver>().AsImplementedInterfaces().SingleInstance();
		}
	}
}