using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cardinal_System_Shared
{

    public abstract class Message
    {
        public long SourceId;
        public long TargetId;
        public MessageType Type;

        protected Message(MessageType type)
        {
            Type = type;
        }

        public MessageDto ToDto()
        {
            return new MessageDto
            {
                Family = Type.GetMessageFamily(),
                Type = Type,
                Message = JsonConvert.SerializeObject(this),
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
                case MessageType.PhysicalEntityPosition:
                case MessageType.PhysicalEntityCreate:
                    return MessageFamily.PhysicalEntity;
                case MessageType.RegisterEntityOwner:
                case MessageType.RegisterEntityInterest:
                case MessageType.RegisterWithCircuit:
                case MessageType.UnregisterEntityInterest:
                case MessageType.UnregisterEntityOwner:
                case MessageType.UnregisterWithCircuit:
                    return MessageFamily.Component;
                default:
                    return MessageFamily.Unknown;
            }
        }
    }

    public enum MessageType
    {
        PhysicalEntityPosition,
        PhysicalEntityCreate,
        RegisterWithCircuit,
        UnregisterWithCircuit,
        RegisterEntityInterest,
        UnregisterEntityInterest,
        RegisterEntityOwner,
        UnregisterEntityOwner,
    }

    public class MessageDto
    {
        public MessageFamily Family { get; set; }
        public MessageType Type { get; set; }
        public long SourceId { get; set; }
        public long TargetId { get; set; }
        public string Message { get; set; }


        public Message TranslateFromDto()
        {
            switch (Type)
            {
                case MessageType.PhysicalEntityPosition:
                    return JsonConvert.DeserializeObject<PhysicalMovementMessage>(Message);
                case MessageType.PhysicalEntityCreate:
                    return JsonConvert.DeserializeObject<PhysicalCreateMessage>(Message);
                case MessageType.RegisterEntityInterest:
                    return JsonConvert.DeserializeObject<RegisterEntityInterestMessage>(Message);
                case MessageType.RegisterWithCircuit:
                case MessageType.UnregisterWithCircuit:
                case MessageType.UnregisterEntityInterest:
                case MessageType.RegisterEntityOwner:
                case MessageType.UnregisterEntityOwner:
                    return JsonConvert.DeserializeObject<ComponentMessage>(Message);
                default:
                    return null;
            }
        }
    }

    public class MessageDtoArray
    {
        public List<MessageDto> MessageDtos { get; set; }
    }
}