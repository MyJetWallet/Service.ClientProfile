using System;
using System.Runtime.Serialization;

namespace Service.ClientProfile.Domain.Models
{
    [DataContract]
    public class Blocker : ICloneable
    {       
        [DataMember(Order = 1)] public int BlockerId { get; set; }
        [DataMember(Order = 2)] public BlockingType BlockedOperationType { get; set; }
        [DataMember(Order = 3)] public DateTime ExpiryTime { get; set; }
        [DataMember(Order = 4)] public string Reason { get; set; }
        [DataMember(Order = 5)] public DateTime LastTs { get; set; }
        public ClientProfile Profile { get; set; }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}