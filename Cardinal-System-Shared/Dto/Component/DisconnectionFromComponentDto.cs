using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class DisconnectionFromComponentDto : MessageDto
    {
        public long ComponentIdConnectionFrom { get; set; }

        public override Message TranslateFromDto()
        {
            return new DisconnectionFromComponent(ComponentIdConnectionFrom)
            {
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                Type = Type,
            };
        }
    }
}