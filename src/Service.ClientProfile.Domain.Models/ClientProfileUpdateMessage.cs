using System.Runtime.Serialization;

namespace Service.ClientProfile.Domain.Models
{
    [DataContract]
    public class ClientProfileUpdateMessage
    {
        public const string TopicName = "jet-wallet-client-profile-update";

        [DataMember (Order = 1)] public ClientProfile OldProfile { get; set; }
        [DataMember (Order = 2)] public ClientProfile NewProfile { get; set; }
    }
}