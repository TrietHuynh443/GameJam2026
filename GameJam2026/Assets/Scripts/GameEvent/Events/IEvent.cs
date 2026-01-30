using Human;

namespace GameEvent.Events
{
    public interface IEvent
    {
        
    }

    public struct NextDaysEvent : IEvent
    {
        public int Days { get; set; }
    }
    
    public struct EntityFightEvent : IEvent
    {
        public HumanAngry HumanAngry;

        public EntityFightEvent(HumanAngry humanAngry)
        {
            HumanAngry = humanAngry;
        }
    }
}