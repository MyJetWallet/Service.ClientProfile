using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Postgres;
using Service.ClientProfile.Services;

namespace Service.ClientProfile.Jobs
{
    public class ReviewSignalJob
    {
        private readonly IServiceBusPublisher<AskForReviewSignal> _publisher;
        private readonly ClientProfileService _clientProfileService;

        public ReviewSignalJob(ISubscriber<AskForReviewAction> subscriber, IServiceBusPublisher<AskForReviewSignal> publisher, ClientProfileService clientProfileService, DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _publisher = publisher;
            _clientProfileService = clientProfileService;
            subscriber.Subscribe(HandleEvents);
        }

        private async ValueTask HandleEvents(AskForReviewAction message)
        {
            var shouldAsk = await _clientProfileService.CheckIsShouldAskAndUpdateProfile(message.ClientId);
            if(!shouldAsk)
                return;

            await _publisher.PublishAsync(new AskForReviewSignal() {ClientId = message.ClientId});
        }
    }
}