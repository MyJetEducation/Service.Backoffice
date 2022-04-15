using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.TcpClient;
using Service.EducationProgress.Client;
using Service.KeyValue.Client;
using Service.MarketProduct.Client;
using Service.PersonalData.Client;
using Service.ServerKeyValue.Client;
using Service.ServiceBus.Models;
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

			var tcpServiceBus = new MyServiceBusTcpClient(() => Program.Settings.ServiceBusWriter, "MyJetEducation Service.Backoffice");
			builder
				.Register(_ => new MyServiceBusPublisher<ClearEducationProgressServiceBusModel>(tcpServiceBus, ClearEducationProgressServiceBusModel.TopicName, false))
				.As<IServiceBusPublisher<ClearEducationProgressServiceBusModel>>()
				.SingleInstance();
			builder
				.Register(_ => new MyServiceBusPublisher<ClearEducationUiProgressServiceBusModel>(tcpServiceBus, ClearEducationUiProgressServiceBusModel.TopicName, false))
				.As<IServiceBusPublisher<ClearEducationUiProgressServiceBusModel>>()
				.SingleInstance();
			tcpServiceBus.Start();
		}
	}
}