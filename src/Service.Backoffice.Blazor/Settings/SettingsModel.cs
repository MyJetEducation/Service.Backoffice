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

        [YamlProperty("Backoffice.UserAccountServiceUrl")]
        public string UserAccountServiceUrl { get; set; }

        [YamlProperty("Backoffice.KeyValueServiceUrl")]
        public string KeyValueServiceUrl { get; set; }

        [YamlProperty("Backoffice.UserInfoCrudServiceUrl")]
        public string UserInfoCrudServiceUrl { get; set; }

        [YamlProperty("Backoffice.EducationProgressServiceUrl")]
        public string EducationProgressServiceUrl { get; set; }

        [YamlProperty("Backoffice.ServerKeyValueServiceUrl")]
        public string ServerKeyValueServiceUrl { get; set; }

        [YamlProperty("Backoffice.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }

        [YamlProperty("Backoffice.KeyEducationProgress")]
        public string KeyEducationProgress { get; set; }
    }
}
