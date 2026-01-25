using GameEvent.Events;
using UnityEditor;
using UnityEngine;

namespace Extension
{
    public static class TestExtension
    {
        [MenuItem("Debug Tools/Trigger Next Day")]
        public static void NextDay()
        {
            GameEvent.GameEvent.Publish(new NextDaysEvent()
            {
                Days = 1
            });
            Debug.Log("trigger next day");
        }
    }
}