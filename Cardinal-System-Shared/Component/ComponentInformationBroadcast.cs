using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class ComponentInformationBroadcast : ComponentMessage
    {
        public long ComponentId { get; private set; }
        public ComponentType ComponentType { get; private set; }

        public ComponentInformationBroadcast(long componentId, ComponentType componentType)
            : base(MessageType.ComponentInformationBroadcast)
        {
            ComponentId = componentId;
            ComponentType = componentType;
        }

        public override MessageDto ToDto()
        {
            return new ComponentInformationBroadcastDto
            {
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                Type = Type,
                ComponentId = ComponentId,
                ComponentType = ComponentType
            };
        }
    }
}