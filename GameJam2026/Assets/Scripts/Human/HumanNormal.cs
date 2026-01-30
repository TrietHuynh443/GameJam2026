using System;
using UnityEngine;
using GameEvent.Events;
using Random = UnityEngine.Random;

namespace Human
{
    public class HumanNormal : MonoBehaviour, IHuman
    {
        public bool isMasked = false;
        
        [Header("Anger")]
        [Range(0f, 1f)]
        public float becomeAngryChance = 0.1f;
    
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
        private HumanAngry _angryHuman;

        private void Start()
        {
            _sickHuman = _transform.GetComponentInChildren<SickHuman>(includeInactive: true);
            _angryHuman = _transform.GetComponentInChildren<HumanAngry>(includeInactive: true);
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
            if (obj.Human != gameObject)
            {
                return;
            }

            if (isMasked)
            {
                isMasked = false;
                Debug.Log("My mask effect wear off!");
                GameEvent.GameEvent.Publish(new ScoreEvent(0, -1));
                return;
            }
            
            Debug.Log("Infected ");
            gameObject.SetActive(false);
            _sickHuman.gameObject.SetActive(true);
            GameEvent.GameEvent.Publish(new ScoreEvent(1, 0));

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
            
            if (Random.value < 0.001f)
            {
                BecomeAngry();
            }
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
            Debug.Log("I wear a mask!");
            GameEvent.GameEvent.Publish(new ScoreEvent(0, 1));
        }

        public void Back()
        {
            _isFaceWall = true;
        }
        
        private void BecomeAngry()
        {
            if (_angryHuman == null)
                return;

            if (_angryHuman.gameObject.activeSelf)
                return;

            if (Random.value > becomeAngryChance)
                return;

            Debug.Log($"{name} became ANGRY 😡");

            _angryHuman.gameObject.SetActive(true);
            _angryHuman.isMasked = isMasked;
            gameObject.SetActive(false);
        }
    }
    
}
