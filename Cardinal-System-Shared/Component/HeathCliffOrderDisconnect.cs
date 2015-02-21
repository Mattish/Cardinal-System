using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffOrderDisconnect : Message
    {
        public HeathCliffOrderDisconnect()
            : base(MessageType.HeathCliffOrderDisconnect)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffOrderDisconnectDto
            {
                Family = Type.GetMessageFamily(),
                Type = Type,
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime
            };
        }
    }
}