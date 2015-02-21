using Cardinal_System_Shared.Dto;
using Cardinal_System_Shared.Dto.Component;

namespace Cardinal_System_Shared.Component
{
    public class HeathCliffNewIdResponse : Message
    {
        public long NewId { get; set; }
        public HeathCliffNewIdResponse(long newId)
            : base(MessageType.HeathCliffNewIdResponse)
        {
            NewId = newId;
        }

        public override MessageDto ToDto()
        {
            return new HeathCliffNewIdResponseDto
            {
                F = Type.GetMessageFamily(),
                T = Type,
                SI = SourceId,
                TI = TargetId,
                SC = SourceComponent,
                TC = TargetComponent,
                CT = CreatedTime,
                NewId = NewId
            };
        }
    }
}