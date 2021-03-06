using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffNewIdResponseDto : MessageDto
    {
        public long NewId { get; set; }

        public override Message TranslateFromDto()
        {
            return new HeathCliffNewIdResponse(NewId)
            {
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                Type = Type,
                NewId = NewId
            };
        }
    }
}