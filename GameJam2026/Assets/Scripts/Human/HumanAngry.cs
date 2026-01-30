using GameEvent.Events;
using UnityEngine;

namespace Human
{
    public class HumanAngry : MonoBehaviour, IHuman
    {
        public bool isMasked = false;
    
        [Header("Movement")]
        public float moveSpeed = 10f;
        public float changeDirectionTime = 5f;
        public Vector2 minBounds = new Vector2(-80, -40);
        public Vector2 maxBounds = new Vector2(80, 40);

        private HumanDirectionType _moveDirection;
        private float _timer;
        private bool _isFaceWall = false;
        [SerializeField] private Transform _transform;
        private SickHuman _sickHuman;
        private HumanNormal _normalHuman;
        
        [Header("Block Mask Params")]
        public float angryDuration = 5f;
        
        private void Start()
        {
            _sickHuman = _transform.GetComponentInChildren<SickHuman>(includeInactive: true);
            _normalHuman = _transform.GetComponentInChildren<HumanNormal>(includeInactive: true);
            StopAllCoroutines();
            StartCoroutine(CalmDownAfterTime());
        }
        
        private void OnEnable()
        {
            GameEvent.GameEvent.Subscribe<EntityFightEvent>(Fight);
            GameEvent.GameEvent.Subscribe<InfectedEvent>(Infected);

        }

        private void OnDisable()
        {
            GameEvent.GameEvent.Unsubscribe<EntityFightEvent>(Fight);
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

        public void Back()
        {
            _isFaceWall = true;
        }
        
        private void Fight(EntityFightEvent evt)
        {
            if (evt.HumanAngry != gameObject)
                return;

            if (isMasked)
            {
                Debug.Log($"{name} already 😡 wear a mask but your insist make him throw it away");
                isMasked = false;
                return;
            }

            Debug.Log($"{name} is too ANGRY 😡 to wear a mask!");
        }

        private System.Collections.IEnumerator CalmDownAfterTime()
        {
            yield return new WaitForSeconds(angryDuration);
            Debug.Log($"{name} calmed down 😮‍💨");
            _normalHuman.gameObject.SetActive(true);
            _normalHuman.isMasked = isMasked;
            gameObject.SetActive(false);
        }
    }
}