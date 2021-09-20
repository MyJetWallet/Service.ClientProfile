using System.Threading.Tasks;
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
    }
}