using System;
using System.Runtime.Serialization;
using Service.ClientProfile.Domain.Models;

namespace Service.ClientProfile.Grpc.Models.Requests
{
    [DataContract]
    public class SubmitReviewRequest
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public int Stars { get; set; }
        [DataMember(Order = 3)] public ReviewAction Action { get; set; }
        [DataMember(Order = 4)] public string ReviewText { get; set; }    
    }
}