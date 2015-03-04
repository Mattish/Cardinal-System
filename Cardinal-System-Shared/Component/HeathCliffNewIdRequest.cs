using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffNewIdRequest : ComponentMessage
    {
        public ComponentType ComponentType { get; private set; }
        public string IpAddress { get; private set; }
        public int Port { get; private set; }
        public HeathCliffNewIdRequest(ComponentType componentType, string ipAddress, int port)
            : base(MessageType.HeathCliffNewIdRequest)
        {
            ComponentType = componentType;
            IpAddress = ipAddress;
            Port = port;
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffNewIdRequestDto
            {
                Family = Type.GetMessageFamily(),
                Type = Type,
                SourceId = SourceId,
                TargetId = TargetId,
                SourceComponent = SourceComponent,
                TargetComponent = TargetComponent,
                CreatedTime = CreatedTime,
                ComponentType = ComponentType,
                IpAddress = IpAddress,
                Port = Port
            };
        }
    }
}