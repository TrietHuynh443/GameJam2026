using Human;
using UnityEngine;

namespace GameEvent.Events
{
    public interface IEvent
    {
        
    }
    
    public struct ScoreEvent : IEvent
    {
        public int Infected;
        public int Masked;

        public ScoreEvent(int infected, int masked)
        {
            Infected = infected;
            Masked = masked;
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
}