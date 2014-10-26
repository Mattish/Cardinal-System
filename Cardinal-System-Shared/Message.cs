using System.Collections.Generic;

namespace Cardinal_System_Shared
{
    public abstract class Message
    {
        public long SourceId;
        public long TargetId;

        public abstract MessageType GetMessageType();
    }

    public enum MessageType
    {
        PhysicalEntityPosition,
        PhysicalEntityCreate,
        EntityInterest,
        RegisterWithCircuit
    }

    public class MessageDto
    {
        public MessageType Type { get; set; }
        public long SourceId { get; set; }
        public long TargetId { get; set; }
        public string Message { get; set; }
    }

    public class MessageDtoArray
    {
        public List<MessageDto> MessageDtos { get; set; }
    }

    public class MessageWrapper
    {
        public MessageType Type { get; set; }
        public Message Message { get; set; }
    }
}