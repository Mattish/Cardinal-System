using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffOrderConnectDto : MessageDto
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public long ComponentId { get; set; }
        public ComponentType ComponentType { get; set; }

        public override Message TranslateFromDto()
        {
            return new HeathCliffOrderConnect(IpAddress, Port, ComponentId, ComponentType)
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