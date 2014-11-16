using System;
using Cardinal_System_Shared.Dtos;

namespace Cardinal_System_Shared.Entity
{
    public class InformationEntity : Entity
    {
        public override EntityType GetEntityType()
        {
            return EntityType.BasicInformationEntity;
        }

        public override void UpdateState(object updateWith, MessageType changeType)
        {
            throw new NotImplementedException();
        }
    }
}