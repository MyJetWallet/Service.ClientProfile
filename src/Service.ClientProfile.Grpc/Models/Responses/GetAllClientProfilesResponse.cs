using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.ClientProfile.Grpc.Models.Responses
{
    [DataContract]
    public class GetAllClientProfilesResponse
    {
        [DataMember(Order = 1)] public List<Domain.Models.ClientProfile> ClientProfiles { get; set; }
    }
}