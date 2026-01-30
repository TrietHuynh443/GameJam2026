using System;
using UnityEngine;
using GameEvent.Events;
using Random = UnityEngine.Random;

namespace Human
{
    public class HumanNormal : MonoBehaviour, IHuman
    {
        public bool isMasked = false;
    
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float changeDirectionTime = 10f;
        public Vector2 minBounds = new Vector2(-80, -40);
        public Vector2 maxBounds = new Vector2(80, 40);

        private HumanDirectionType _moveDirection;
        private float _timer;
        private bool _isFaceWall = false;
        [SerializeField] private Transform _transform;
        private SickHuman _sickHuman;

        private void Start()
        {
            _sickHuman = _transform.GetComponentInChildren<SickHuman>(includeInactive: true);
        }

        private void OnEnable()
        {
            GameEvent.GameEvent.Subscribe<EntityMaskedEvent>(WearMask);
            GameEvent.GameEvent.Subscribe<InfectedEvent>(Infected);
        }

        
        private void OnDisable()
        {
            GameEvent.GameEvent.Unsubscribe<EntityMaskedEvent>(WearMask);
            GameEvent.GameEvent.Unsubscribe<InfectedEvent>(Infected);
            
        }
        private void Infected(InfectedEvent obj)
        {
            Debug.Log("Infected ");
            if (obj.Human != gameObject)
            {
                return;
            }

            if (isMasked)
            {
                isMasked = false;
                return;
            }

            gameObject.SetActive(false);
            _sickHuman.gameObject.SetActive(true);
        }
        
        void FixedUpdate()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= changeDirectionTime)
            {
                _moveDirection = GetDirection();
                _timer = 0f;
            }

            Vector3 nextPos = transform.position + (Vector3)(HumanDirectionExtension.DirectionMap[_moveDirection] * moveSpeed * Time.fixedDeltaTime);
            
            if (nextPos.x < minBounds.x || nextPos.x > maxBounds.x ||
                nextPos.y < minBounds.y || nextPos.y > maxBounds.y)
            {
                _moveDirection = GetDirection();
                return;
            }

            _transform.position = nextPos;
        }

        private HumanDirectionType GetDirection()
        {
            if (_isFaceWall)
            {
                _isFaceWall = false;
                return HumanDirectionExtension.GetReverseDirection(_moveDirection);
            }

            return (HumanDirectionType)Random.Range(0, 7);
        }
        private void WearMask(EntityMaskedEvent evt)
        {
            if (evt.HumanNormal != gameObject)
            {
                return;
            }
        
            if (isMasked)
            {
                Debug.Log("I already wear a mask!");
                return;
            }
        
            isMasked = true;
            GameEvent.GameEvent.Publish(new ScoreEvent());
        }

        public void Back()
        {
            _isFaceWall = true;
        }
    }
    
}
