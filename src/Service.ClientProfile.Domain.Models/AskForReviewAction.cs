using System.Runtime.Serialization;

namespace Service.ClientProfile.Domain.Models
{
    [DataContract]
    public class AskForReviewAction
    {
        public const string TopicName = "jet-wallet-review-action";
        [DataMember(Order = 1)] public string ClientId { get; set; }
    }
}