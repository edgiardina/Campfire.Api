namespace Campfire.Api.Models
{

    /*{TextMessage || PasteMessage || SoundMessage || AdvertisementMessage ||
      AllowGuestsMessage || DisallowGuestsMessage || IdleMessage || KickMessage ||
      LeaveMessage || EnterMessage || SystemMessage || TimestampMessage ||
      TopicChangeMessage || UnidleMessage || LockMessage || UnlockMessage ||
      UploadMessage || ConferenceCreatedMessage || ConferenceFinishedMessage}*/
    public enum MessageType
    {
        TextMessage,
        PasteMessage,
        SoundMessage,
        TweetMessage,
        TimestampMessage,
        KickMessage,
        EnterMessage,
        LeaveMessage,
        UploadMessage,
        LockMessage,
        UnlockMessage,
        TopicChangeMessage
    }
}
