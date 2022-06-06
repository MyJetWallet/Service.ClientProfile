using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Service.ClientProfile.Grpc.Models;
using Service.ClientProfile.Grpc.Models.Requests;
using Service.ClientProfile.Grpc.Models.Requests.Blockers;
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
        Task<GetAllClientProfilesResponse> GetAllProfilesPaged(GetAllRequest request);
        
        [OperationContract]
        IAsyncEnumerable<BlockerGrpcModel> GetClientBlockers(GetClientProfileBlockersRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> SetKYCPassed(SetKYCPassedRequest request);
        
        [OperationContract]
        Task<ClientByReferralResponse> GetProfileByReferralCode(GetProfileByReferralCodeRequest request);
        
        [OperationContract]
        Task<GetAllClientProfilesResponse> GetReferrals(GetReferralsRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> AddReferral(AddReferralRequest request);

        [OperationContract]
        Task<ClientProfileUpdateResponse> ChangeReferralCode(ChangeReferralCodeRequest request);
        
        [OperationContract]
        Task<GetAllClientProfilesResponse> GetProfileByExternalId(GetClientByExternalIdRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> SetMarketingEmailSettings(SetMarketingEmailSettingsRequest request);

        [OperationContract]
        Task<ClientProfileUpdateResponse> SetUserType(SetUserTypeRequest request);
        
        [OperationContract]
        Task<ClientProfileUpdateResponse> SetInternalSimpleEmailAsync(SetInternalSimpleEmailRequest request);
    }
}