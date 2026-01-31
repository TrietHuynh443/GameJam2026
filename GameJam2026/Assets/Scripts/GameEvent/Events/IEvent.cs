using Human;
using UnityEngine;

namespace GameEvent.Events
{
    public interface IEvent
    {
        
    }
    
    public struct ScoreEvent : IEvent
    {
        public int Normal;
        public int Infected;
        public int Masked;

        public ScoreEvent(int infected, int masked, int normal)
        {
            Infected = infected;
            Masked = masked;
            Normal = normal;
        }
    }

    public struct NextDaysEvent : IEvent
    {
        public int Days { get; set; }
    }
    
    public struct EntityMaskedEvent : IEvent
    {
        public GameObject HumanNormal;

        public EntityMaskedEvent(GameObject humanNormal)
        {
            HumanNormal = humanNormal;
        }
    }

    public struct EntityFightEvent : IEvent
    {
        public GameObject HumanAngry;

        public EntityFightEvent(GameObject humanAngry)
        {
            HumanAngry = humanAngry;
        }
    }

    public struct InfectedEvent : IEvent
    {
        public GameObject Human;

        public InfectedEvent(GameObject human)
        {
            Human = human;
        }
    }
    
    public struct EntityDragEvent : IEvent
    {
        public GameObject Source;
        public GameObject Target;

        public EntityDragEvent(GameObject source, GameObject target)
        {
            Source = source;
            Target = target;
        }
    }
    
    public struct EntityStopDragEvent : IEvent
    {
        public GameObject Target;

        public EntityStopDragEvent(GameObject target)
        {
            Target = target;
        }
    }

}