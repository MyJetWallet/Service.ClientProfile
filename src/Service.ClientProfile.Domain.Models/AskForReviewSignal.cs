using System.Runtime.Serialization;

namespace Service.ClientProfile.Domain.Models
{
    [DataContract]
    public class AskForReviewSignal
    {
        public const string TopicName = "jet-wallet-review-signal";
        [DataMember(Order = 1)] public string ClientId { get; set; }
    }
}