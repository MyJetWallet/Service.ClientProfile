using System;
using System.Runtime.Serialization;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class AddBlockerToClientRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public string BlockerReason { get; set; }
        [DataMember(Order = 3)] public BlockingType Type { get; set; }
        [DataMember(Order = 4)] public DateTime ExpiryTime { get; set; }
    }
}