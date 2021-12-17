using Autofac;
using Service.KeyValue.Client;
using Service.UserInfo.Crud.Client;
using Service.UserProfile.Client;

namespace Service.Backoffice.Blazor.Modules ;

	public class ClientModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl);
			builder.RegisterKeyValueClient(Program.Settings.KeyValueServiceUrl);
			builder.RegisterUserProfileClient(Program.Settings.UserProfileServiceUrl);
		}
	}