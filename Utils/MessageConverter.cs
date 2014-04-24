using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Campfire.Api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Campfire.Api.Utils
{
    /// <summary>
    /// Helper class for Json Deserialization of Campfire Messages
    /// </summary>
    internal class MessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Message) == (objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType,
            object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var messageType = (MessageType)Enum.Parse(typeof(MessageType), (string)jObject["type"]);

            Message target;

            switch (messageType)
            {
                case MessageType.UploadMessage:
                    target = new UploadMessage();
                    break;
                case MessageType.TweetMessage:
                    target = new TweetMessage();
                    break;
                case MessageType.SoundMessage:
                    target = new SoundMessage();
                    break;
                default:
                    target = new Message();
                    break;
            }

            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}
