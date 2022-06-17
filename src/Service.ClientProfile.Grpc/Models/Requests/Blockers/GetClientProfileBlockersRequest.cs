using System.Runtime.Serialization;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Grpc.Models.Requests.Blockers;

[DataContract]
public class GetClientProfileBlockersRequest
{
    [DataMember(Order = 1)] public BlockingType? Type { get; set; }

    [DataMember(Order = 2)] public string ClientId { get; set; }
}