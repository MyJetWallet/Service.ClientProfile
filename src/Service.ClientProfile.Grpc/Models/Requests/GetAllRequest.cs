using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class GetAllRequest
    {
        [DataMember(Order = 1)] public int Skip { get; set; }
        [DataMember(Order = 2)] public int Take { get; set; }
    }
}