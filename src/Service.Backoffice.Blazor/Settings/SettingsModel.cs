using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.Backoffice.Blazor.Settings
{
    public class SettingsModel
    {
        [YamlProperty("Backoffice.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("Backoffice.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("Backoffice.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("Backoffice.UserProfileServiceUrl")]
        public string UserProfileServiceUrl { get; set; }

        [YamlProperty("Backoffice.KeyValueServiceUrl")]
        public string KeyValueServiceUrl { get; set; }

        [YamlProperty("Backoffice.UserInfoCrudServiceUrl")]
        public string UserInfoCrudServiceUrl { get; set; }

        [YamlProperty("Backoffice.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
    }
}
