using UnityEngine;

namespace Trigger
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerObject : MonoBehaviour
    {
        public TriggerEventType triggerType;
        public bool isAuto = true;

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (!isAuto)
                return;
            RaiseEvent(other, TriggerPhase.Enter);
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {
            if (!isAuto)
                return;
            RaiseEvent(other, TriggerPhase.Stay);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            if (!isAuto)
                return;
            RaiseEvent(other, TriggerPhase.Exit);
        }

        protected void RaiseEvent(Collider2D other, TriggerPhase phase)
        {
            TriggerEvent evt = new TriggerEvent(
                this,
                other.gameObject,
                triggerType,
                phase
            );

            GameEvent.GameEvent.Publish(evt);
        }
    }
}

