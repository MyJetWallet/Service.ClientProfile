using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests;

[DataContract]
public class SetInternalSimpleEmailRequest
{
    [DataMember(Order = 1)] public string ClientId { get; set; }
    [DataMember(Order = 2)] public string InternalSimpleEmail { get; set; }
}