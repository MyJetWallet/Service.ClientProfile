using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Service.ClientProfile.Domain.Models
{
    [DataContract]
    public class ClientProfile : ICloneable
    {
        [DataMember(Order = 1)] public string ClientId { get; set; }
        [DataMember(Order = 2)] public Status2FA Status2FA { get; set; }
        [DataMember(Order = 3)] public List<Blocker> Blockers { get; set; }
        [DataMember(Order = 4)] public bool EmailConfirmed { get; set; }
        [DataMember(Order = 5)] public bool PhoneConfirmed { get; set; }
        [DataMember(Order = 6)] public bool KYCPassed { get; set; }

        public object Clone()
        {
            var profile = (ClientProfile)MemberwiseClone();
            profile.Blockers = Blockers?.Select(b=>(Blocker)b.Clone()).ToList() ?? new List<Blocker>();
            return profile;
        }
    }
}