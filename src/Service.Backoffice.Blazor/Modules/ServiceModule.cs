using Autofac;
using Service.Backoffice.Blazor.Services;

namespace Service.Backoffice.Blazor.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<UserDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<KeyValueDataService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<EmailSenderOperationService>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterType<EducationProgressService>().AsImplementedInterfaces().SingleInstance();
		}
	}
}