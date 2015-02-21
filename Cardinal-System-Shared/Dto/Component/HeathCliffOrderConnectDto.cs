using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffOrderConnectDto : MessageDto
    {
        public override Message TranslateFromDto()
        {
            return new HeathCliffOrderConnect
            {
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