using System;
using System.Collections.Generic;
using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Entity;

namespace Cardinal_System_Shared
{
    public abstract class Message
    {
        public EntityId SourceId;
        public EntityId TargetId;
        public long SourceComponent;
        public long TargetComponent;
        public MessageType Type;
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

        public abstract MessageDto ToDto();
    }

    public class MessageDtoArray
    {
        public List<MessageDto> Dtos { get; set; }
    }
}