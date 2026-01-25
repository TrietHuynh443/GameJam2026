using GameEvent.Events;
using UnityCommunity.UnitySingleton;

namespace Times
{
    public class TimeSystem : PersistentMonoSingleton<TimeSystem>
    {
        private int _currentDay = 1;
        public int CurrentDay => _currentDay;

        public void NextDays(int days = 1)
        {
            _currentDay += days;
            GameEvent.GameEvent.Publish(new NextDaysEvent()
            {
                Days = days
            });
        }
    }
}