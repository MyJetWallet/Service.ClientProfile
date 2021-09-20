using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class Disable2FaRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
    }
}