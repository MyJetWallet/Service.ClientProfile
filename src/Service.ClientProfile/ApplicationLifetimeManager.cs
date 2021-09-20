using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.NoSql;
using MyJetWallet.Sdk.Service;
using MyServiceBus.TcpClient;
using Service.ClientProfile.Jobs;

namespace Service.ClientProfile
{
    public class ApplicationLifetimeManager : ApplicationLifetimeManagerBase
    {
        private readonly ILogger<ApplicationLifetimeManager> _logger;
        private readonly MyServiceBusTcpClient[] _myServiceBusTcpClients;
        private readonly ExpirationCheckJob _expirationCheckJob;
        public ApplicationLifetimeManager(IHostApplicationLifetime appLifetime, ILogger<ApplicationLifetimeManager> logger, MyServiceBusTcpClient[] myServiceBusTcpClients, ExpirationCheckJob expirationCheckJob)
            : base(appLifetime)
        {
            _logger = logger;
            _myServiceBusTcpClients = myServiceBusTcpClients;
            _expirationCheckJob = expirationCheckJob;
        }

        protected override void OnStarted()
        {
            _logger.LogInformation("OnStarted has been called.");
            foreach (var client in _myServiceBusTcpClients)
            {
                client.Start();
            }
            _expirationCheckJob.Start();
        }

        protected override void OnStopping()
        {
            _logger.LogInformation("OnStopping has been called."); 
            foreach (var client in _myServiceBusTcpClients)
            {
                client.Stop();
            }      
            _expirationCheckJob.Dispose();
        }

        protected override void OnStopped()
        {
            _logger.LogInformation("OnStopped has been called.");
        }
    }
}
