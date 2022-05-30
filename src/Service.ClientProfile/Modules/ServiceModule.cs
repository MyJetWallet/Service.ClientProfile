using Autofac;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Jobs;
using Service.ClientProfile.Services;
using Service.PersonalData.Client;

namespace Service.ClientProfile.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var spotServiceBusClient = builder
                .RegisterMyServiceBusTcpClient(() => Program.Settings.SpotServiceBusHostPort, Program.LogFactory);
            
            builder.RegisterMyServiceBusPublisher<ClientProfileUpdateMessage>(spotServiceBusClient, ClientProfileUpdateMessage.TopicName, false);
            builder.RegisterMyNoSqlWriter<ClientProfileNoSqlEntity>(() => Program.Settings.MyNoSqlWriterUrl, ClientProfileNoSqlEntity.TableName);
            
            var queueName = "Spot-Client-Profile-Service";
            builder.RegisterPersonalDataClient(Program.Settings.PersonalDataServiceUrl);
            builder.RegisterPersonalDataUpdateSubscriber(spotServiceBusClient, queueName);
            builder.RegisterType<ClientProfileService>().AsSelf().SingleInstance();
            builder.RegisterType<ProfileCacheManager>().AsSelf().SingleInstance();
            builder.RegisterType<ExpirationCheckJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<ProfileUpdaterJob>().AsSelf().AutoActivate().SingleInstance();
        }
    }
}