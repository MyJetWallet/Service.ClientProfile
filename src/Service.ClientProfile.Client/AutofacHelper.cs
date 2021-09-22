using Autofac;
using MyNoSqlServer.Abstractions;
using MyNoSqlServer.DataReader;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.ClientProfile.Client
{
    public static class AutofacHelper
    {        /// <summary>
        /// Register interfaces:
        ///   * IClientProfileService
        /// </summary>
        public static void RegisterClientProfileClientWithoutCache(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new ClientProfileClientFactory(grpcServiceUrl, null);

            builder.RegisterInstance(factory.GetClientProfileService()).As<IClientProfileService>().SingleInstance();
        }
        
        /// <summary>
        /// Register interfaces:
        ///   * IClientProfileService
        /// </summary>
        public static void RegisterClientProfileClients(this ContainerBuilder builder, IMyNoSqlSubscriber myNoSqlSubscriber, string grpcServiceUrl)
        {
            var subs = new MyNoSqlReadRepository<ClientProfileNoSqlEntity>(myNoSqlSubscriber, ClientProfileNoSqlEntity.TableName);

            var factory = new ClientProfileClientFactory(grpcServiceUrl, subs);

            builder
                .RegisterInstance(subs)
                .As<IMyNoSqlServerDataReader<ClientProfileNoSqlEntity>>()
                .SingleInstance();

            builder
                .RegisterInstance(factory.GetClientProfileService())
                .As<IClientProfileService>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
