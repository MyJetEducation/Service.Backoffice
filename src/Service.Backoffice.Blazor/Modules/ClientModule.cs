using Autofac;
using Service.EducationProgress.Client;
using Service.KeyValue.Client;
using Service.MarketProduct.Client;
using Service.ServerKeyValue.Client;
using Service.TokenRate.Client;
using Service.UserAccount.Client;
using Service.UserInfo.Crud.Client;
using Service.UserTokenAccount.Client;

namespace Service.Backoffice.Blazor.Modules
{
	public class ClientModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof (ServerKeyValueClientFactory)));
			builder.RegisterUserInfoCrudClient(Program.Settings.UserInfoCrudServiceUrl, Program.LogFactory.CreateLogger(typeof (UserInfoCrudClientFactory)));
			builder.RegisterTokenRateClient(Program.Settings.TokenRateServiceUrl, Program.LogFactory.CreateLogger(typeof (TokenRateClientFactory)));
			builder.RegisterUserAccountClient(Program.Settings.UserAccountServiceUrl, Program.LogFactory.CreateLogger(typeof (UserAccountClientFactory)));
			builder.RegisterUserTokenAccountClient(Program.Settings.UserTokenAccountServiceUrl, Program.LogFactory.CreateLogger(typeof (UserTokenAccountClientFactory)));
			builder.RegisterMarketProductClient(Program.Settings.MarketProductServiceUrl, Program.LogFactory.CreateLogger(typeof (MarketProductClientFactory)));

			builder.RegisterKeyValueClient(Program.Settings.KeyValueServiceUrl);
			builder.RegisterEducationProgressClient(Program.Settings.EducationProgressServiceUrl);
		}
	}
}