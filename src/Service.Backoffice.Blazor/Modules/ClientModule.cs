using Autofac;
using Service.EducationProgress.Client;
using Service.KeyValue.Client;
using Service.ServerKeyValue.Client;
using Service.UserInfo.Crud.Client;
using Service.UserProfile.Client;

namespace Service.Backoffice.Blazor.Modules
{
	public class ClientModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof(ServerKeyValueClientFactory)));

			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl);
			builder.RegisterKeyValueClient(Program.Settings.KeyValueServiceUrl);
			builder.RegisterUserProfileClient(Program.Settings.UserProfileServiceUrl);
			builder.RegisterEducationProgressClient(Program.Settings.EducationProgressServiceUrl);
		}
	}
}