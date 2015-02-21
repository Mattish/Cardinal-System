using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffNewIdRequest : Message
    {
        public HeathCliffNewIdRequest()
            : base(MessageType.HeathCliffNewIdRequest)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffNewIdRequestDto
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