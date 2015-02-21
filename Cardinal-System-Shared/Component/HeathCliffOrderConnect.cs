using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffOrderConnect : Message
    {
        public HeathCliffOrderConnect()
            : base(MessageType.HeathCliffOrderConnect)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffOrderConnectDto
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