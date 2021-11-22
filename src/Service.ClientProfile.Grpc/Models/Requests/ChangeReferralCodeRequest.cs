using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class ChangeReferralCodeRequest
    {
        [DataMember(Order = 1)] public string ReferralCode { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
    }
}