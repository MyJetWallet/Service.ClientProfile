using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Responses
{
    [DataContract]
    public class ClientByReferralResponse
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public bool IsExists { get; set; }
    }
}