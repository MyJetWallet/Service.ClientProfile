using System.ServiceModel;
using System.Threading.Tasks;
using Service.ClientProfile.Grpc.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Grpc.Models.Responses;

namespace Service.ClientProfile.Grpc
{
    [ServiceContract]
    public interface IClientProfileService
    {
        [OperationContract]
        Task<ClientProfileUpdateResponse> AddBlockerToClient(AddBlockerToClientRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> DeleteBlockerFromClient(DeleteBlockerFromClientRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> Enable2Fa(Enable2FaRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> Disable2Fa(Disable2FaRequest request);
        
        [OperationContract]
        Task<Domain.Models.ClientProfile> GetOrCreateProfile(GetClientProfileRequest request);

        [OperationContract]
        Task<GetAllClientProfilesResponse> GetAllProfiles();
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> SetKYCPassed(SetKYCPassedRequest request);
    }
}