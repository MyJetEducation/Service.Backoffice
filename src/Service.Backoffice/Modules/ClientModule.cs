using Autofac;
using Service.EducationProgress.Client;
using Service.KeyValue.Client;
using Service.MarketProduct.Client;
using Service.PersonalData.Client;
using Service.ServerKeyValue.Client;
using Service.UserTokenAccount.Client;

namespace Service.Backoffice.Modules
{
	public class ClientModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterServerKeyValueClient(Program.Settings.ServerKeyValueServiceUrl, Program.LogFactory.CreateLogger(typeof (ServerKeyValueClientFactory)));
			builder.RegisterUserTokenAccountClient(Program.Settings.UserTokenAccountServiceUrl, Program.LogFactory.CreateLogger(typeof (UserTokenAccountClientFactory)));
			builder.RegisterMarketProductClient(Program.Settings.MarketProductServiceUrl, Program.LogFactory.CreateLogger(typeof (MarketProductClientFactory)));
			builder.RegisterPersonalDataClient(Program.Settings.PersonalDataServiceUrl);
			builder.RegisterKeyValueClient(Program.Settings.KeyValueServiceUrl);
			builder.RegisterEducationProgressClient(Program.Settings.EducationProgressServiceUrl);
		}
	}
}