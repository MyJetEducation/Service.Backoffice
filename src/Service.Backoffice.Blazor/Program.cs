using System.Net;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using MyJetWallet.Sdk.Service;
using MySettingsReader;
using Service.Backoffice.Blazor.Settings;
using Service.Core.Client.Constants;
using Service.Core.Client.Helpers;

namespace Service.Backoffice.Blazor
{
	public class Program
	{
		public static string ApplicationTitle = $"{Configuration.ProductName} Backoffice";

		public static SettingsModel Settings { get; private set; }

		public static ILoggerFactory LogFactory { get; private set; }

		public static void Main(string[] args)
		{
			Console.Title = "MyJetEducation Service.Backoffice";
			Settings = LoadSettings();

			using ILoggerFactory loggerFactory = LogConfigurator.ConfigureElk(Configuration.ProductName, Settings.SeqServiceUrl, Settings.ElkLogs);
			LogFactory = loggerFactory;

			CreateHostBuilder(loggerFactory, args);
		}

		private static SettingsModel LoadSettings() => SettingsReader.GetSettings<SettingsModel>(ProgramHelper.SettingsFileName);

		public static Func<T> ReloadedSettings<T>(Func<SettingsModel, T> getter) => () => getter.Invoke(LoadSettings());

		public static void CreateHostBuilder(ILoggerFactory loggerFactory, string[] args)
		{
			ILogger<Program> logger = loggerFactory.CreateLogger<Program>();

			try
			{
				logger.LogInformation("Application is being started");

				Host.CreateDefaultBuilder(args)
					.UseServiceProviderFactory(new AutofacServiceProviderFactory())
					.ConfigureWebHostDefaults(webBuilder =>
						webBuilder.ConfigureKestrel(options =>
						{
							options.Listen(IPAddress.Any, ProgramHelper.LoadPort("HTTP_PORT", "8080"), o => o.Protocols = HttpProtocols.Http1);
							options.Listen(IPAddress.Any, ProgramHelper.LoadPort("GRPC_PORT", "80"), o => o.Protocols = HttpProtocols.Http2);
						}).UseStartup<Startup>())
					.ConfigureServices(services => services
						.AddSingleton(loggerFactory)
						.AddSingleton(typeof (ILogger<>), typeof (Logger<>)))
					.Build().Run();

				logger.LogInformation("Application has been stopped");
			}
			catch (Exception ex)
			{
				logger.LogCritical(ex, "Application has been terminated unexpectedly");
			}
		}
	}
}