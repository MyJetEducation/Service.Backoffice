using Autofac;
using Service.Backoffice.Blazor.Services;

namespace Service.Backoffice.Blazor.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<UserInfoDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<KeyValueDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<EmailSenderOperationDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<EducationProgressDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<RetryDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<TokenRateDataService>().AsImplementedInterfaces().SingleInstance();
		}
	}
}