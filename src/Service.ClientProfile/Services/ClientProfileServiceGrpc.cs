using System.Collections.Generic;
using System.Threading.Tasks;
using Service.ClientProfile.Domain.Models;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Grpc.Models.Responses;

namespace Service.ClientProfile.Services
{
    public class ClientProfileServiceGrpc : IClientProfileService
    {
        private readonly ClientProfileService _clientProfileService;

        public ClientProfileServiceGrpc(ClientProfileService clientProfileService)
        {
            _clientProfileService = clientProfileService;
        }

        public async Task<ClientProfileUpdateResponse> AddBlockerToClient(AddBlockerToClientRequest request) => await _clientProfileService.AddBlockerToClient(request);

        public async Task<ClientProfileUpdateResponse> DeleteBlockerFromClient(DeleteBlockerFromClientRequest request) => await _clientProfileService.DeleteBlockerFromClient(request);

        public async Task<ClientProfileUpdateResponse> Enable2Fa(Enable2FaRequest request) => await _clientProfileService.Enable2Fa(request);

        public async Task<ClientProfileUpdateResponse> Disable2Fa(Disable2FaRequest request) => await _clientProfileService.Disable2Fa(request);

        public async Task<Domain.Models.ClientProfile> GetOrCreateProfile(GetClientProfileRequest request) => await _clientProfileService.GetOrCreateProfile(request);

        public async Task<GetAllClientProfilesResponse> GetAllProfiles() => await _clientProfileService.GetAllProfiles();
        public IAsyncEnumerable<BlockerGrpcModel> GetClientBlockers() => _clientProfileService.GetClientProfileBlockers();

        public async Task<ClientProfileUpdateResponse> SetKYCPassed(SetKYCPassedRequest request) =>
           await _clientProfileService.SetKYCPassed(request);

        public async Task<ClientByReferralResponse> GetProfileByReferralCode(GetProfileByReferralCodeRequest request) => await _clientProfileService.GetProfileByReferralCode(request.ReferralCode);
        public async Task<GetAllClientProfilesResponse> GetReferrals(GetReferralsRequest request)  => await _clientProfileService.GetReferrals(request.ClientId);

        public async Task<ClientProfileUpdateResponse> AddReferral(AddReferralRequest request) => await _clientProfileService.AddReferral(request);

        public async Task<ClientProfileUpdateResponse> ChangeReferralCode(ChangeReferralCodeRequest request) => await _clientProfileService.ChangeReferralCode(request);

        public async Task<GetAllClientProfilesResponse> GetProfileByExternalId(GetClientByExternalIdRequest request) =>
            await _clientProfileService.GetProfileByExternalId(request.SearchText);

        public async Task<ClientProfileUpdateResponse> SetMarketingEmailSettings(SetMarketingEmailSettingsRequest request) => await _clientProfileService.SetMarketingEmailSettings(request);
    }
}