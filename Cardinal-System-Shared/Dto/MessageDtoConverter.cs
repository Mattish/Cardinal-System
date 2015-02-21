using System;
using Cardinal_System_Shared.Dto.Component;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cardinal_System_Shared.Dto
{
    public class MessageDtoConverter : JsonCreationConverter<MessageDto>
    {
        protected override MessageDto Create(Type objectType, JObject jObject)
        {
            var jTokenType = jObject["T"]; //Type
            var messageTypeInt = jTokenType.Value<int>();
            MessageType messageType = (MessageType)messageTypeInt;
            MessageDto returnDto;
            switch (messageType)
            {
                case MessageType.HeathCliffOrderConnect:
                    returnDto = jObject.ToObject<HeathCliffOrderConnectDto>();
                    break;
                case MessageType.HeathCliffOrderDisconnect:
                    returnDto = jObject.ToObject<HeathCliffOrderDisconnectDto>();
                    break;
                case MessageType.HeathCliffNewIdRequest:
                    returnDto = jObject.ToObject<HeathCliffNewIdRequestDto>();
                    break;
                case MessageType.HeathCliffNewIdResponse:
                    returnDto = jObject.ToObject<HeathCliffNewIdResponseDto>();
                    break;
                case MessageType.Heartbeat:
                    returnDto = jObject.ToObject<HeartbeatDto>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return returnDto;
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            T target = Create(objectType, jObject);
            serializer.Populate(jObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
