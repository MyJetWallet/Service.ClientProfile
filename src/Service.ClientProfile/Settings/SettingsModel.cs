using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.ClientProfile.Settings
{
    public class SettingsModel
    {
        [YamlProperty("ClientProfile.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("ClientProfile.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("ClientProfile.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }
        
        [YamlProperty("ClientProfile.MyNoSqlWriterUrl")]
        public string MyNoSqlWriterUrl { get; set; }
        
        [YamlProperty("ClientProfile.SpotServiceBusHostPort")]
        public string SpotServiceBusHostPort { get; set; }
        
        [YamlProperty("ClientProfile.PostgresConnectionString")]
        public string PostgresConnectionString { get; set; }
        
        [YamlProperty("ClientProfile.MaxCachedEntities")]
        public int MaxCachedEntities { get; set; }
        
        [YamlProperty("ClientProfile.ExpirationCheckTimerInMin")]
        public int ExpirationCheckTimerInMin { get; set; }
        
        [YamlProperty("ClientProfile.PersonalDataServiceBusHostPort")]
        public string PersonalDataServiceBusHostPort { get; set; }
        
        [YamlProperty("ClientProfile.PersonalDataServiceUrl")]
        public string PersonalDataServiceUrl { get; set; }
        
    }
}
