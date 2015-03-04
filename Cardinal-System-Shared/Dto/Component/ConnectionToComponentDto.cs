using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class ConnectionToComponentDto : MessageDto
    {
        public long ComponentIdConnectionTo { get; set; }

        public override Message TranslateFromDto()
        {
            return new ConnectionToComponent(ComponentIdConnectionTo)
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