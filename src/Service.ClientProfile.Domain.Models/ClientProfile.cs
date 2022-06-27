using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MyJetWallet.ServiceBus.SessionAudit.Models;

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
        [DataMember(Order = 7)] public string ReferralCode { get; set; }
        [DataMember(Order = 8)] public string ReferrerClientId { get; set; }
        [DataMember(Order = 9)] public DateTime LastTs { get; set; }
        [DataMember(Order = 10)] public string ExternalClientId { get; set; }
        [DataMember(Order = 11)] public bool MarketingEmailAllowed { get; set; }
        [DataMember(Order = 12)] public UserType UserType { get; set; }
        [DataMember(Order = 13)] public string InternalSimpleEmail { get; set; }
        [DataMember(Order = 14)] public bool AskToSubmitReview { get; set; }
        [DataMember(Order = 15)] public bool? IsMobile { get; set; }
	    [DataMember(Order = 16)] public DeviceOperationSystem DeviceOperationSystem { get; set; }

        public object Clone()
        {
            var profile = (ClientProfile)MemberwiseClone();
            profile.Blockers = Blockers?.Select(b=>(Blocker)b.Clone()).ToList() ?? new List<Blocker>();
            return profile;
        }
    }
}