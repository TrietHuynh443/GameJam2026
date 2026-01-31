using UnityCommunity.UnitySingleton;
using UnityEngine;
using Human;
using GameEvent.Events;

namespace Trigger
{
    [DefaultExecutionOrder(-40)]
    public class TriggerManager : MonoSingleton<TriggerManager>
    {
        // ----------------------------------
        // Lifecycle
        // ----------------------------------
        protected override void OnInitialized()
        {
            GameEvent.GameEvent.Subscribe<TriggerEvent>(OnTriggerEvent);
        }

        public override void ClearSingleton()
        {
            GameEvent.GameEvent.Unsubscribe<TriggerEvent>(OnTriggerEvent);
        }

        // ----------------------------------
        // Event Entry Point
        // ----------------------------------
        private void OnTriggerEvent(TriggerEvent evt)
        {
            HandleTriggerEvent(evt);
        }

        // ----------------------------------
        // Routing
        // ----------------------------------
        private void HandleTriggerEvent(TriggerEvent evt)
        {
            switch (evt.EventType)
            {
                case TriggerEventType.ApplyMask:
                    HandleApplyMask(evt);
                    break;
                case TriggerEventType.Fight:
                    HandleFight(evt);
                    break;
                case TriggerEventType.Infect:
                    HandleInfected(evt);
                    break;
                default:
                    Debug.LogWarning($"Unhandled TriggerEventType: {evt.EventType}");
                    break;
            }
        }



        #region Handlers
        private void HandleInfected(TriggerEvent evt)
        {
            if (evt.Phase != TriggerPhase.Enter)
                return;
            GameEvent.GameEvent.Publish(new InfectedEvent(evt.TriggeredObject));

        }
        private void HandleApplyMask(TriggerEvent evt)
        {
            if (evt.Phase != TriggerPhase.Stay)
                return;

            GameEvent.GameEvent.Publish(new EntityMaskedEvent(evt.TriggeredObject));
        }
        
        private void HandleFight(TriggerEvent evt)
        {
            if (evt.Phase != TriggerPhase.Enter)
                return;

            GameEvent.GameEvent.Publish(new EntityFightEvent(evt.TriggeredObject));
        }

        #endregion
    }
}
