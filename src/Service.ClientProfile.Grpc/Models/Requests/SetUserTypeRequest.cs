using System.Runtime.Serialization;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class SetUserTypeRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public UserType UserType { get; set; }
    }
}