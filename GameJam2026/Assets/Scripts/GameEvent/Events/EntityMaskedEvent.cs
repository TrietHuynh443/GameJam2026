using Human;
using UnityEngine;

namespace GameEvent.Events
{
    public struct EntityMaskedEvent : IEvent
    {
        public GameObject HumanNormal;

        public EntityMaskedEvent(GameObject humanNormal)
        {
            HumanNormal = humanNormal;
        }
    }
}
