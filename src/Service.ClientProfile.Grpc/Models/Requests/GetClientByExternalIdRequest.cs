using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class GetClientByExternalIdRequest
    {
        [DataMember(Order = 1)] public string ExternalClientId { get; set; }
    }
}