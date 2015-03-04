using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class DisconnectionFromComponent : ComponentMessage
    {
        public long ComponentIdConnectionFrom { get; private set; }

        public DisconnectionFromComponent(long componentIdConnectionFrom)
            : base(MessageType.DisconnectedFromComponent)
        {
            ComponentIdConnectionFrom = componentIdConnectionFrom;
        }

        public override MessageDto ToDto()
        {
            return new DisconnectionFromComponentDto()
            {
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                Type = Type,
                ComponentIdConnectionFrom = ComponentIdConnectionFrom
            };
        }
    }
}