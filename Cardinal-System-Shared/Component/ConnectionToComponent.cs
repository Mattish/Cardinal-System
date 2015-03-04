using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class ConnectionToComponent : ComponentMessage
    {
        public long ComponentIdConnectionTo { get; private set; }

        public ConnectionToComponent(long componentIdConnectionTo)
            : base(MessageType.ConnectedToComponent)
        {
            ComponentIdConnectionTo = componentIdConnectionTo;
        }

        public override MessageDto ToDto()
        {
            return new ConnectionToComponentDto()
            {
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                Type = Type,
                ComponentIdConnectionTo = ComponentIdConnectionTo
            };
        }
    }
}