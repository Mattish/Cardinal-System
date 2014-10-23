using System.Collections.Generic;

namespace Cardinal_System_Shared
{
    public abstract class Message
    {
        public int SourceId;
        public int TargetId;

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
        public int SourceId { get; set; }
        public int TargetId { get; set; }
        public string Message { get; set; }
    }

    public class MessageDtoArray
    {
        public IEnumerable<MessageDto> MessageDtos { get; set; }
    }

    public class MessageWrapper
    {
        public MessageType Type { get; set; }
        public Message Message { get; set; }
    }
}