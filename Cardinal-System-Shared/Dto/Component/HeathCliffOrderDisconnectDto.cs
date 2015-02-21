using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffOrderDisconnectDto : MessageDto
    {
        public override Message TranslateFromDto()
        {
            return new HeathCliffOrderDisconnect
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