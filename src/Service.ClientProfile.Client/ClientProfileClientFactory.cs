using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using MyJetWallet.Sdk.GrpcMetrics;
using MyNoSqlServer.DataReader;
using ProtoBuf.Grpc.Client;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Grpc;

namespace Service.ClientProfile.Client
{
    [UsedImplicitly]
    public class ClientProfileClientFactory
    {      
        private readonly MyNoSqlReadRepository<ClientProfileNoSqlEntity> _reader;
        private readonly CallInvoker _channel;
        public ClientProfileClientFactory(string grpcServiceUrl, MyNoSqlReadRepository<ClientProfileNoSqlEntity> reader)
        {
            _reader = reader;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var channel = GrpcChannel.ForAddress(grpcServiceUrl);
            _channel = channel.Intercept(new PrometheusMetricsInterceptor());
        }

        public IClientProfileService GetClientProfileService() =>
            _reader != null 
        ? new NoSqlClientProfileClient(_channel.CreateGrpcService<IClientProfileService>(), _reader)
        :  _channel.CreateGrpcService<IClientProfileService>();
    }
}
