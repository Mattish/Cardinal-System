using System;
using System.Collections.Generic;
using Cardinal_System_Shared.Dtos.Component;
using Cardinal_System_Shared.Entity;
using Newtonsoft.Json;

namespace Cardinal_System_Shared.Dtos
{
    public abstract class Message
    {
        public EntityId SourceId;
        public EntityId TargetId;
        public MessageType Type;
        public object MessageObj;
        public DateTime CreatedTime;

        private Message()
        {
            CreatedTime = DateTime.UtcNow;
        }

        protected Message(MessageType type)
            : this()
        {
            Type = type;
        }

        public MessageDto ToDto()
        {
            return new MessageDto
            {
                Family = Type.GetMessageFamily(),
                Type = Type,
                MessageObj = JsonConvert.SerializeObject(MessageObj),
                SourceId = SourceId,
                TargetId = TargetId
            };
        }
    }

    public enum MessageFamily
    {
        PhysicalEntity,
        Component,
        Unknown,
    }

    public static class MessageTypeExtensions
    {
        public static MessageFamily GetMessageFamily(this MessageType type)
        {
            switch (type)
            {
                case MessageType.InitialInfoRequest:
                case MessageType.InitialInfoResponse:
                    return MessageFamily.Component;
                default:
                    return MessageFamily.Unknown;
            }
        }
    }

    public enum MessageType
    {
        InitialInfoRequest,
        InitialInfoResponse
    }

    public class MessageDto
    {
        public MessageFamily Family { get; set; }
        public MessageType Type { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntityId SourceId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public EntityId TargetId { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long SourceComponent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long TargetComponent { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MessageObj { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime CreatedTime { get; set; }

        public Message TranslateFromDto()
        {
            switch (Type)
            {
                case MessageType.InitialInfoRequest:
                    return new InitialInfoRequestMessage
                    {
                        MessageObj = MessageObj,
                        SourceId = SourceId,
                        TargetId = TargetId,
                        CreatedTime = CreatedTime,
                        Type = Type
                    };
                case MessageType.InitialInfoResponse:
                    return new InitialInfoResponseMessage
                    {
                        MessageObj = MessageObj,
                        SourceId = SourceId,
                        TargetId = TargetId,
                        CreatedTime = CreatedTime,
                        Type = Type
                    };
                default:
                    return null;
            }
        }
    }

    public class MessageDtoArray
    {
        public List<MessageDto> Dtos { get; set; }
    }
}