using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class ComponentInformationBroadcastDto : MessageDto
    {
        public long ComponentId { get; set; }
        public ComponentType ComponentType { get; set; }

        public override Message TranslateFromDto()
        {
            return new ComponentInformationBroadcast(ComponentId, ComponentType)
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