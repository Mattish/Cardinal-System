using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffOrderDisconnect : Message
    {
        public HeathCliffOrderDisconnect()
            : base(MessageType.HeathCliffOrderDisconnect)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffOrderDisconnectDto
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