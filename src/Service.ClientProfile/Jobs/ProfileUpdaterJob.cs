using System;
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
    public class ProfileUpdaterJob {
        private readonly IPersonalDataServiceGrpc _personalDataService;
        private readonly ClientProfileService _clientProfileService;
        
        public ProfileUpdaterJob(IPersonalDataServiceGrpc personalDataService,
            ISubscriber<ITraderUpdate> personalDataSubscriber, ClientProfileService clientProfileService)
        {
            _personalDataService = personalDataService;
            _clientProfileService = clientProfileService;
            personalDataSubscriber.Subscribe(UpdateProfileStatusesIfNeeded);
        }

        private async ValueTask UpdateProfileStatusesIfNeeded(ITraderUpdate traderUpdate)
        {
            var pd = await _personalDataService.GetByIdAsync(traderUpdate.TraderId);
            if(pd.PersonalData == null)
                return;

            var client = await _clientProfileService.GetOrCreateProfile(new GetClientProfileRequest()
            {
                ClientId = traderUpdate.TraderId
            });

            var confirmPhone = pd.PersonalData.ConfirmPhone != null;
            var confirmEmail = pd.PersonalData.Confirm != null;
            
            if (confirmPhone)
            {
                if (client.Status2FA == Status2FA.NotSet)
                {
                    await _clientProfileService.Enable2Fa(new Enable2FaRequest()
                    {
                        ClientId = traderUpdate.TraderId
                    });
                }
            }
            if (client.PhoneConfirmed != confirmPhone || client.EmailConfirmed != confirmEmail)
            {
                await _clientProfileService.UpdateConfirmStatuses(traderUpdate.TraderId, confirmPhone, confirmEmail);
            }
        }
    }
}