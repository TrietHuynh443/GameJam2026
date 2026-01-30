using Human;

namespace GameEvent.Events
{
    public struct EntityMaskedEvent : IEvent
    {
        public HumanNormal HumanNormal;

        public EntityMaskedEvent(HumanNormal humanNormal)
        {
            HumanNormal = humanNormal;
        }
    }
}
