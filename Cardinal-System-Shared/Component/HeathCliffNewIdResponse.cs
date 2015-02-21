using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffNewIdResponse : Message
    {
        public HeathCliffNewIdResponse()
            : base(MessageType.HeathCliffNewIdResponse)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffNewIdResponseDto
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