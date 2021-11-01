using System.Linq;
using System.Threading.Tasks;
using MyNoSqlServer.DataReader;
using Service.ClientProfile.Domain.Models.NoSql;
using Service.ClientProfile.Grpc;
using Service.ClientProfile.Grpc.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Grpc.Models.Responses;

namespace Service.ClientProfile.Client
{
    public class NoSqlClientProfileClient : IClientProfileService
    {
        
        private readonly IClientProfileService _grpcService;
        private readonly MyNoSqlReadRepository<ClientProfileNoSqlEntity> _reader;

        public NoSqlClientProfileClient(IClientProfileService grpcService, MyNoSqlReadRepository<ClientProfileNoSqlEntity> reader)
        {
            _reader = reader;
            _grpcService = grpcService;
        }

        public async Task<ClientProfileUpdateResponse> AddBlockerToClient(AddBlockerToClientRequest request)=> await _grpcService.AddBlockerToClient(request);

        public async Task<ClientProfileUpdateResponse> DeleteBlockerFromClient(DeleteBlockerFromClientRequest request)=> await _grpcService.DeleteBlockerFromClient(request);

        public async Task<ClientProfileUpdateResponse> Enable2Fa(Enable2FaRequest request)=> await _grpcService.Enable2Fa(request);

        public async Task<ClientProfileUpdateResponse> Disable2Fa(Disable2FaRequest request) => await _grpcService.Disable2Fa(request);

        public async Task<Domain.Models.ClientProfile> GetOrCreateProfile(GetClientProfileRequest request)
        {
            var entity = _reader.Get(ClientProfileNoSqlEntity.GeneratePartitionKey(),
                ClientProfileNoSqlEntity.GenerateRowKey(request.ClientId));
            if (entity != null)
                return entity.ClientProfile;

            return await _grpcService.GetOrCreateProfile(request);
        }

        public async Task<GetAllClientProfilesResponse> GetAllProfiles()
        {
            var entities = _reader.Get();
            return new GetAllClientProfilesResponse()
            {
                ClientProfiles = entities.Select(t => t.ClientProfile).ToList()
            };
        }

        public async Task<ClientProfileUpdateResponse> SetKYCPassed(SetKYCPassedRequest request) => await _grpcService.SetKYCPassed(request);
        
        public async Task<ClientByReferralResponse> GetProfileByReferralCode(GetProfileByReferralCodeRequest request) => await _grpcService.GetProfileByReferralCode(request);
    }
}