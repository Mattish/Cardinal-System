using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffOrderConnect : ComponentMessage
    {
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public long ComponentId { get; private set; }
        public ComponentType ComponentType { get; private set; }

        public HeathCliffOrderConnect(string ipAddress, int port, long componentId, ComponentType type)
            : base(MessageType.HeathCliffOrderConnect)
        {
            IpAddress = ipAddress;
            Port = port;
            ComponentId = componentId;
            ComponentType = type;
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffOrderConnectDto
            {
                Family = Type.GetMessageFamily(),
                Type = Type,
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                IpAddress = IpAddress,
                Port = Port,
                ComponentId = ComponentId,
                ComponentType = ComponentType
            };
        }
    }
}