using UnityEngine;
using GameEvent.Events;

namespace Human
{
    public class HumanNormal : MonoBehaviour
    {
        public bool isMasked = false;
    
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float changeDirectionTime = 10f;
        public Vector2 minBounds = new Vector2(-80, -40);
        public Vector2 maxBounds = new Vector2(80, 40);

        private Vector2 _moveDirection;
        private float _timer;
    
        private static readonly Vector2[] Directions =
        {
            new Vector2( 1,  0),
            new Vector2(-1,  0),
            new Vector2( 0,  1),
            new Vector2( 0, -1),
            new Vector2( 1,  1),
            new Vector2( 1, -1),
            new Vector2(-1,  1),
            new Vector2(-1, -1),
        };
        
        private void OnEnable()
        {
            GameEvent.GameEvent.Subscribe<EntityMaskedEvent>(WearMask);
        }

        private void OnDisable()
        {
            GameEvent.GameEvent.Unsubscribe<EntityMaskedEvent>(WearMask);
        }

        void Start()
        {
            PickRandomDirection();
        }

        void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= changeDirectionTime)
            {
                PickRandomDirection();
                _timer = 0f;
            }

            Vector3 nextPos = transform.position + (Vector3)(_moveDirection * moveSpeed * Time.fixedDeltaTime);

            if (nextPos.x < minBounds.x || nextPos.x > maxBounds.x ||
                nextPos.y < minBounds.y || nextPos.y > maxBounds.y)
            {
                PickRandomDirection();
                return;
            }

            transform.position = nextPos;
        }

        private void PickRandomDirection()
        {
            _moveDirection = Directions[Random.Range(0, Directions.Length)].normalized;
        }

        private void WearMask(EntityMaskedEvent evt)
        {
            if (evt.HumanNormal != this)
            {
                return;
            }
        
            if (isMasked)
            {
                Debug.Log("I already wear a mask!");
                return;
            }
        
            Debug.Log("You give me mask!");
            isMasked = true;
            GameEvent.GameEvent.Publish(new ScoreEvent(evt.HumanNormal));

        }
    }
    
}
