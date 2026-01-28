using UnityCommunity.UnitySingleton;
using UnityEngine;

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
                case TriggerEventType.Door:
                    HandleDoor(evt);
                    break;

                case TriggerEventType.DamageZone:
                    HandleDamageZone(evt);
                    break;

                case TriggerEventType.Dialogue:
                    HandleDialogue(evt);
                    break;

                case TriggerEventType.Checkpoint:
                    HandleCheckpoint(evt);
                    break;

                case TriggerEventType.Point:
                    HandlePoint(evt);
                    break;

                default:
                    Debug.LogWarning($"Unhandled TriggerEventType: {evt.EventType}");
                    break;
            }
        }

        #region Handlers

        private void HandlePoint(TriggerEvent evt)
        {
            if (evt.Phase != TriggerPhase.Enter)
                return;

            Debug.Log("Point acquired");
            GameEvent.GameEvent.Publish(new EntityConsumedEvent(evt.Trigger.gameObject, evt.TriggeredObject.gameObject));
        }

        private void HandleDoor(TriggerEvent evt)
        {
            if (evt.Phase == TriggerPhase.Enter)
                Debug.Log("Door opened");

            if (evt.Phase == TriggerPhase.Exit)
                Debug.Log("Door closed");
        }

        private void HandleDamageZone(TriggerEvent evt)
        {
            if (evt.Phase == TriggerPhase.Enter)
                Debug.Log("Start apply damage");

            if (evt.Phase == TriggerPhase.Stay)
                Debug.Log("Applying damage");

            if (evt.Phase == TriggerPhase.Exit)
                Debug.Log("Stop apply damage");
        }

        private void HandleDialogue(TriggerEvent evt)
        {
            if (evt.Phase == TriggerPhase.Enter)
                Debug.Log("Start dialogue");

            if (evt.Phase == TriggerPhase.Exit)
                Debug.Log("End dialogue");
        }

        private void HandleCheckpoint(TriggerEvent evt)
        {
            if (evt.Phase == TriggerPhase.Enter)
                Debug.Log("Checkpoint saved");
        }

        #endregion
    }
}
