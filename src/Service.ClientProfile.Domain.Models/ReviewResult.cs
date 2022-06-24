using System;
using System.Runtime.Serialization;

namespace Service.ClientProfile.Domain.Models
{
    [DataContract]
    public class ReviewResult
    {
        [DataMember(Order = 1)] public int Id { get; set; }
        [DataMember(Order = 2)] public string ClientId { get; set; }
        [DataMember(Order = 3)] public DateTime Timestamp { get; set; }
        [DataMember(Order = 4)] public int Stars { get; set; }
        [DataMember(Order = 5)] public ReviewAction Action { get; set; }
        [DataMember(Order = 6)] public string ReviewText { get; set; }
    }
}