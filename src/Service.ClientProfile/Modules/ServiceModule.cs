using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using DotNetCoreDecorators;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.ServiceBus;
using MyNoSqlServer.DataReader;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Jobs;
using Service.ClientProfile.Services;
using SimpleTrading.PersonalData.Abstractions.PersonalDataUpdate;
using SimpleTrading.PersonalData.Grpc;
using SimpleTrading.PersonalData.ServiceBus.PersonalDataUpdate;

namespace Service.ClientProfile.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var spotServiceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.SpotServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory);
            builder.RegisterMyServiceBusPublisher<ClientProfileUpdateMessage>(spotServiceBusClient, ClientProfileUpdateMessage.TopicName, false);
            builder.RegisterMyNoSqlWriter<ClientProfileNoSqlEntity>(Program.ReloadedSettings(e => e.MyNoSqlWriterUrl), ClientProfileNoSqlEntity.TableName);
            
            var queueName = "Spot-Client-Profile-Service";
            var serviceBusClient = MyServiceBusTcpClientFactory.Create(Program.ReloadedSettings(e => e.PersonalDataServiceBusHostPort), ApplicationEnvironment.HostName, Program.LogFactory.CreateLogger("PersonalDataServiceBus"));
            builder.RegisterInstance(serviceBusClient).SingleInstance();
            
            builder.RegisterInstance(new PersonalDataUpdateMyServiceBusSubscriber(serviceBusClient, queueName, TopicQueueType.Permanent, false))
                .As<ISubscriber<ITraderUpdate>>()
                .SingleInstance();
            
            var personalDataClientFactory = new MyGrpcClientFactory(Program.Settings.PersonalDataServiceUrl);
            builder
                .RegisterInstance(personalDataClientFactory.CreateGrpcService<IPersonalDataServiceGrpc>())
                .As<IPersonalDataServiceGrpc>()
                .SingleInstance();
            
            builder.RegisterType<ClientProfileService>().AsSelf().SingleInstance();
            builder.RegisterType<ProfileCacheManager>().AsSelf().SingleInstance();
            builder.RegisterType<ExpirationCheckJob>().AsSelf().AutoActivate().SingleInstance();
            builder.RegisterType<TwoFaUpdaterJob>().AsSelf().AutoActivate().SingleInstance();
        }
    }
}