using System;
using System.Runtime.Serialization;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Grpc.Models;

[DataContract]
public class BlockerGrpcModel
{
    [DataMember(Order = 1)] public int BlockerId { get; set; }
    [DataMember(Order = 2)] public BlockingType BlockedOperationType { get; set; }
    [DataMember(Order = 3)] public DateTime ExpiryTime { get; set; }
    [DataMember(Order = 4)] public string Reason { get; set; }
    [DataMember(Order = 5)] public DateTime LastTs { get; set; }
    [DataMember(Order = 6)] public string ClientId { get; set; }
}