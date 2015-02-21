using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffNewIdRequest : Message
    {
        public HeathCliffNewIdRequest()
            : base(MessageType.HeathCliffNewIdRequest)
        {
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffNewIdRequestDto
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