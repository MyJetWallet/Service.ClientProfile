using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class GetProfileByReferralCodeRequest
    {
        [DataMember(Order = 1)] public string ReferralCode { get; set; }
    }
}