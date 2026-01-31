using UnityEngine;
using System.Collections.Generic;

namespace Trigger
{
    [RequireComponent(typeof(Collider2D))]
    public class TriggerObject : MonoBehaviour
    {
        public bool isAuto = true;

        protected readonly List<ITriggerAction> actions = new();

        protected virtual void OnEnable()
        {
            actions.Clear();

            var components = GetComponents<MonoBehaviour>();
            foreach (var comp in components)
            {
                if (comp is ITriggerAction action)
                {
                    actions.Add(action);
                }
            }
        }

        private void Reset()
        {
            GetComponent<Collider2D>().isTrigger = true;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {

        }

        protected virtual void OnTriggerStay2D(Collider2D other)
        {

        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {

        }

        protected void ExecuteActions(
            Collider2D other,
            TriggerPhase phase,
            TriggerEventType triggerType
        )
        {
            foreach (var action in actions)
            {
                if (action.TriggerType == triggerType)
                {
                    action.Execute(this, other.gameObject, phase);
                }
            }
        }

    }
}
