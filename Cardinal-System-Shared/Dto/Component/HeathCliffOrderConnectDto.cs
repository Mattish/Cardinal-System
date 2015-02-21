using Cardinal_System_Shared.Component;

namespace Cardinal_System_Shared.Dto.Component
{
    public class HeathCliffOrderConnectDto : MessageDto
    {
        public override Message TranslateFromDto()
        {
            return new HeathCliffOrderConnect
            {
                Type = T,
                SourceId = SI,
                TargetId = TI,
                SourceComponent = SC,
                TargetComponent = TC,
                CreatedTime = CT
            };
        }
    }
}