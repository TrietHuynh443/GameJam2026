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
        [SerializeField] private NPCStateController _controller;
        
        [SerializeField] private Animator _animator;
        
        [Header("Block Mask Params")]
        public float angryDuration = 5f;

        private HumanDirectionType _avoidDir = 0;


        private void OnEnable()
        {
            StopAllCoroutines();
            StartCoroutine(CalmDownAfterTime());
            GameEvent.GameEvent.Subscribe<EntityFightEvent>(Fight);
        }

        private void OnDisable()
        {
            GameEvent.GameEvent.Unsubscribe<EntityFightEvent>(Fight);
        }
        public void Infected()
        {
            if (isMasked)
            {
                isMasked = false;
                return;
            }
            
            _controller.SetState(HumanState.Sick);
            
        }

        public void Masked()
        {
            isMasked = true;
        }

        public void RotateAround()
        {
            
        }

        public void Move()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= changeDirectionTime)
            {
                _moveDirection = GetDirection();
                PlayAnimation(_moveDirection);
                _timer = 0f;
            }

            if (_controller.CheckObstacle(HumanDirectionExtension.DirectionMap[_moveDirection]))
            {
                _avoidDir = _moveDirection;
                _moveDirection = GetDirection();
                PlayAnimation(_moveDirection);
                return;
            }
            Vector3 nextPos = transform.position + (Vector3)(HumanDirectionExtension.DirectionMap[_moveDirection] * moveSpeed * Time.fixedDeltaTime);
            
            if (nextPos.x < minBounds.x || nextPos.x > maxBounds.x ||
                nextPos.y < minBounds.y || nextPos.y > maxBounds.y)
            {
                _moveDirection = GetDirection();
                PlayAnimation(_moveDirection);
                return;
            }

            _transform.position = nextPos;
        }

        private HumanDirectionType GetDirection()
        {
            var dir = Random.Range(0, 7);
            if ((HumanDirectionType)dir == _avoidDir)
            {
                dir = (dir + 1) % 8;
            }
            _avoidDir = HumanDirectionType.None;

            return (HumanDirectionType)dir;
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
            
            if (!gameObject.activeSelf) yield break;
            
            _controller.SetState(HumanState.Normal);
        }
        
        private void PlayAnimation(HumanDirectionType direction)
        {
            string animationName = HumanDirectionExtension.DirectionNameMap[direction];
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                _animator.Play(animationName);
            }
        }
    }
}