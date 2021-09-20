using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.DataReader;
using MyServiceBus.TcpClient;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Jobs;
using Service.ClientProfile.Services;

namespace Service.ClientProfile.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<ClientProfileUpdateMessage>(serviceBusClient, ClientProfileUpdateMessage.TopicName, false);
            builder.RegisterMyNoSqlWriter<ClientProfileNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), ClientProfileNoSqlEntity.TableName);
            
            builder.RegisterType<ClientProfileService>().AsSelf().SingleInstance();
            builder.RegisterType<ProfileCacheManager>().AsSelf().SingleInstance();
            builder.RegisterType<ExpirationCheckJob>().AsSelf().AutoActivate().SingleInstance();
        }
    }
}