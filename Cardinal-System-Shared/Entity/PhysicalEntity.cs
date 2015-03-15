using System;

namespace Cardinal_System_Shared.Entity
{
    public class PhysicalEntity : Entity
    {
        private Tuple<float, float> _xyPos;

        public Tuple<float, float> Position
        {
            get { return _xyPos; }
        }

        public PhysicalEntity()
        {
            _xyPos = new Tuple<float, float>(0, 0);
        }

        public PhysicalEntity(EntityId entityId, Tuple<float, float> initialTuple)
            : base(entityId)
        {
            _xyPos = initialTuple;
        }

        public override void UpdateState()
        {
            foreach (var message in MessagesToProcess)
            {
            }
        }

        public override EntityType GetEntityType()
        {
            return EntityType.BasicPhysicalEntity;
        }
    }
}