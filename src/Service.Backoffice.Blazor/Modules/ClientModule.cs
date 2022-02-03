using Autofac;
using Service.EducationProgress.Client;
using Service.KeyValue.Client;
using Service.UserInfo.Crud.Client;
using Service.UserProfile.Client;
using AutofacHelper = Service.ServerKeyValue.Client.AutofacHelper;

namespace Service.Backoffice.Blazor.Modules
{
	public class ClientModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl);
			builder.RegisterKeyValueClient(Program.Settings.KeyValueServiceUrl);
			builder.RegisterUserProfileClient(Program.Settings.UserProfileServiceUrl);
			builder.RegisterEducationProgressClient(Program.Settings.EducationProgressServiceUrl);
			AutofacHelper.RegisterKeyValueClient(builder, Program.Settings.ServerKeyValueServiceUrl);
		}
	}
}