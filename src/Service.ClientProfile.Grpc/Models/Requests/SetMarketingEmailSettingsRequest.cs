using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class SetMarketingEmailSettingsRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public bool IsAllowed { get; set; }

    }
}