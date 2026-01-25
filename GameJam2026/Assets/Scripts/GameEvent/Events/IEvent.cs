namespace GameEvent.Events
{
    public interface IEvent
    {
        
    }

    public struct NextDaysEvent : IEvent
    {
        public int Days { get; set; }
    }
}