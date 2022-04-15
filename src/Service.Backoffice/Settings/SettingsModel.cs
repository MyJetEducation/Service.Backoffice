using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Backoffice.Settings
{
    public class SettingsModel
    {
        [YamlProperty("Backoffice.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("Backoffice.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("Backoffice.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("Backoffice.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("Backoffice.KeyValueServiceUrl")]
        public string KeyValueServiceUrl { get; set; }

        [YamlProperty("Backoffice.EducationProgressServiceUrl")]
        public string EducationProgressServiceUrl { get; set; }

        [YamlProperty("Backoffice.ServerKeyValueServiceUrl")]
        public string ServerKeyValueServiceUrl { get; set; }

        [YamlProperty("Backoffice.UserTokenAccountServiceUrl")]
        public string UserTokenAccountServiceUrl { get; set; }

        [YamlProperty("Backoffice.MarketProductServiceUrl")]
        public string MarketProductServiceUrl { get; set; }

        [YamlProperty("Backoffice.PersonalDataServiceUrl")]
        public string PersonalDataServiceUrl { get; set; }

        [YamlProperty("Backoffice.ServiceBusWriter")]
        public string ServiceBusWriter { get; set; }
    }
}
