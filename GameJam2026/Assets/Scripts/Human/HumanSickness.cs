using GameEvent.Events;
using Trigger;
using UnityEngine;

namespace Human
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SickHuman : TriggerObject, IHuman
    {
        private const float Speed = 0.5f;
        private float MoveDeltaTime => 1f / Speed;
        
        private HumanDirectionType _direction;

        [SerializeField] private Transform _transform;

        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _feverAnimator;
        

        private HumanDirectionType _avoidDir = HumanDirectionType.None;
        [SerializeField] private NPCStateController _controller;
        private float _timer;

        private const float DeadTime = 120f;
        private float _deadTimer;
        private bool _isDead;
        
        protected override void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer("NPC");
            _controller.isMasked = false;
            
            _deadTimer = 0f;
            _isDead = false;
            
            base.OnEnable();
        }


        public void Move()
        {
            if (!_isDead)
            {
                _deadTimer += Time.fixedDeltaTime;

                if (_deadTimer >= DeadTime)
                {
                    _isDead = true;
                    Dead();
                    return;
                }
            }
            
            _timer += Time.fixedDeltaTime;

            if (_timer >= 2)
            {
                _direction = GetDirection();
                PlayAnimation(_direction);
                _timer = 0f;
            }
            
            if (_controller.CheckObstacle(HumanDirectionExtension.DirectionMap[_direction]))
            {
                _avoidDir = _direction;
                _direction = GetDirection();
                PlayAnimation(_direction);
                return;
            }
            Vector3 nextPos = transform.position + (Vector3)(HumanDirectionExtension.DirectionMap[_direction] * (0.5f * Time.fixedDeltaTime));
            
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

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            ExecuteActions(other, TriggerPhase.Enter, TriggerEventType.Infect);
        }

        public void Back()
        {
            
        }
        
        private void PlayAnimation(HumanDirectionType direction)
        {
            string animationName = HumanDirectionExtension.DirectionNameMap[direction];
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                _animator.Play(animationName);
                if (_feverAnimator.gameObject.activeSelf)
                    _feverAnimator.Play(animationName);
            }
        }

        public void Infected()
        {
        }

        public void Masked()
        {
        }

        public void Dead()
        {
            GameEvent.GameEvent.Publish<EntityDeadEvent>(new EntityDeadEvent(this.gameObject));
        }

        public void Cured()
        {
            _deadTimer = 0f;
            _isDead = false;
            
            GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent(-1, 0, 1));
            _controller.SetState(HumanState.Normal);
        }

        public void RotateAround()
        {
            
        }

    }
}
