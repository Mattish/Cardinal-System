using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffNewIdRequestDto : MessageDto
    {
        public override Message TranslateFromDto()
        {
            return new HeathCliffNewIdRequest
            {
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                Type = Type
            };
        }
    }
}