using GameEvent.Events;
using UnityEngine;

namespace Human
{
    public class HumanAngry : HumanNormal
    {
        private bool _canWearMask = true;
        [Range(0f, 1f)]
        public float blockMaskChance = 0.5f;

        public float blockDuration = 3f;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            GameEvent.GameEvent.Subscribe<EntityFightEvent>(Fight);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            GameEvent.GameEvent.Unsubscribe<EntityFightEvent>(Fight);
        }

        protected override void WearMask(EntityMaskedEvent evt)
        {
            if (evt.HumanNormal != this)
                return;

            if (!_canWearMask)
            {
                Debug.Log($"{name} refuses to wear a mask right now");
                return;
            }

            if (isMasked)
            {
                Debug.Log("I already wear a mask!");
                return;
            }

            isMasked = true;
            GameEvent.GameEvent.Publish(new ScoreEvent(this));
        }
        
        private void Fight(EntityFightEvent evt)
        {
            if (evt.HumanAngry != this)
                return;

            Debug.Log($"{name} is ANGRY 😡");

            if (Random.value < blockMaskChance)
            {
                Debug.Log($"{name} is too angry to wear a mask!");
                StopAllCoroutines();
                StartCoroutine(BlockMaskTemporarily());
            }
        }

        private System.Collections.IEnumerator BlockMaskTemporarily()
        {
            _canWearMask = false;
            yield return new WaitForSeconds(blockDuration);
            _canWearMask = true;
            blockMaskChance = 0f;
            Debug.Log($"{name} calmed down 😮‍💨");
        }
    }
}