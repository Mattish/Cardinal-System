using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffOrderConnect : Message
    {
        public HeathCliffOrderConnect()
            : base(MessageType.HeathCliffOrderConnect)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffOrderConnectDto
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