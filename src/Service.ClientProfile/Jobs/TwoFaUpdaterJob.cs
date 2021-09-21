using System.Threading.Tasks;
using DotNetCoreDecorators;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Services;
using SimpleTrading.PersonalData.Abstractions.PersonalData;
using SimpleTrading.PersonalData.Abstractions.PersonalDataUpdate;
using SimpleTrading.PersonalData.Grpc;

namespace Service.ClientProfile.Jobs
{
    public class TwoFaUpdaterJob {
        private readonly IPersonalDataServiceGrpc _personalDataService;
        private readonly ClientProfileService _clientProfileService;
        
        public TwoFaUpdaterJob(IPersonalDataServiceGrpc personalDataService,
            ISubscriber<ITraderUpdate> personalDataSubscriber, ClientProfileService clientProfileService)
        {
            _personalDataService = personalDataService;
            _clientProfileService = clientProfileService;
            personalDataSubscriber.Subscribe(Update2FaStatusIfNeeded);
        }

        private async ValueTask Update2FaStatusIfNeeded(ITraderUpdate traderUpdate)
        {
            var pd = await _personalDataService.GetByIdAsync(traderUpdate.TraderId);
            if(pd.PersonalData == null || string.IsNullOrEmpty(pd.PersonalData.Phone) || pd.PersonalData.ConfirmPhone == null)
                return;

            if (pd.PersonalData.ConfirmPhone != null)
            {
                var client = await _clientProfileService.GetOrCreateProfile(new GetClientProfileRequest()
                {
                    ClientId = traderUpdate.TraderId
                });

                if (client.Status2FA == Status2FA.NotSet)
                {
                    await _clientProfileService.Enable2Fa(new Enable2FaRequest()
                    {
                        ClientId = traderUpdate.TraderId
                    });
                }
            }
        }
    }
}