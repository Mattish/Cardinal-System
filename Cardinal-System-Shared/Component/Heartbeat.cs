using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class Heartbeat : Message
    {
        public Heartbeat()
            : base(MessageType.Heartbeat)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeartbeatDto
            {
                F = Type.GetMessageFamily(),
                T = Type,
                SI = SourceId,
                TI = TargetId,
                SC = SourceComponent,
                TC = TargetComponent,
                CT = CreatedTime
            };
        }
    }
}