using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Responses
{
    [DataContract]
    public class ClientProfileUpdateResponse
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public bool IsSuccess { get; set; }
        [DataMember(Order = 3)] public string Error { get; set; }
    }
}