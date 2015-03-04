using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffNewIdRequestDto : MessageDto
    {
        public ComponentType ComponentType { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }

        public override Message TranslateFromDto()
        {
            return new HeathCliffNewIdRequest(ComponentType, IpAddress, Port)
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