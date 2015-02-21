using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffNewIdRequestDto : MessageDto
    {
        public override Message TranslateFromDto()
        {
            return new HeathCliffNewIdRequest
            {
                SourceId = SI,
                TargetId = TI,
                SourceComponent = SC,
                TargetComponent = TC,
                CreatedTime = CT,
                Type = T
            };
        }
    }
}