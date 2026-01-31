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
        [SerializeField] private Transform _transform;

        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _animatorMask;

        [SerializeField] private NPCStateController _controller;

        public void Infected()
        {
            if (_controller.isMasked)
            {
                Debug.Log("My mask effect wear off!");
                return;
            }
            
            Debug.Log("Infected ");
        }

        public void Masked()
        {
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
        public void Back()
        {
            
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
