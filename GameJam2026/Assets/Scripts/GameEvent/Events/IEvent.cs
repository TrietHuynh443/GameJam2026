using Human;
using UnityEngine;

namespace GameEvent.Events
{
    public interface IEvent
    {
        
    }

    public struct NextDaysEvent : IEvent
    {
        public int Days { get; set; }
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