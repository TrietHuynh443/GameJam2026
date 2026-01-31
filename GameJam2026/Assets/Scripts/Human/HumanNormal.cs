using System;
using UnityEngine;
using GameEvent.Events;
using Random = UnityEngine.Random;

namespace Human
{
    public class HumanNormal : MonoBehaviour, IHuman
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float changeDirectionTime = 10f;
        public Vector2 minBounds = new Vector2(-80, -40);
        public Vector2 maxBounds = new Vector2(80, 40);

        private HumanDirectionType _moveDirection;
        private float _timer;
        private bool _isFaceWall = false;
        [SerializeField] private Transform _transform;

        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _animatorMask;

        [SerializeField] private NPCStateController _controller;

        public void Infected()
        {
            if (_controller.isMasked)
            {
                _controller.isMasked = false;
                Debug.Log("My mask effect wear off!");
                GameEvent.GameEvent.Publish(new ScoreEvent(0, -1));
                return;
            }
            
            Debug.Log("Infected ");
            gameObject.SetActive(false);
            GameEvent.GameEvent.Publish(new ScoreEvent(1, 0));
        }

        public void Masked()
        {
            _controller.isMasked = true;
        }

        public void RotateAround()
        {
        }

        [Header("Anger")]
        [Range(0f, 1f)]
        public float becomeAngryChance = 0.1f;

        private HumanDirectionType _avoidDir = HumanDirectionType.None;

        public void Move()
        {
            _timer += Time.fixedDeltaTime;

            if (_timer >= changeDirectionTime)
            {
                if (Random.Range(0, 1f) < becomeAngryChance)
                {
                    Debug.Log("Human angry");
                    _controller.SetState(HumanState.Angry);
                }
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
            var dir = (HumanDirectionType)Random.Range(0, 7);
            if (dir == _avoidDir)
            {
                dir = HumanDirectionExtension.GetReverseDirection(dir);
            }
            _avoidDir = HumanDirectionType.None;

            return dir;
        }
        private void WearMask(EntityMaskedEvent evt)
        {
            if (evt.HumanNormal != gameObject)
            {
                return;
            }
        
            if (_controller.isMasked)
            {
                Debug.Log("I already wear a mask!");
                return;
            }
        
            _controller.isMasked = true;
            Debug.Log("I wear a mask!");
            GameEvent.GameEvent.Publish(new ScoreEvent(0, 1));
        }

        public void Back()
        {
            _isFaceWall = true;
        }
        
        private void PlayAnimation(HumanDirectionType direction)
        {
            string animationName = HumanDirectionExtension.DirectionNameMap[direction];
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName) )
            {
                _animator.Play(animationName);
                if (_animatorMask.gameObject.activeSelf)
                    _animatorMask.Play(animationName);
            }
        }
    }
    
}
