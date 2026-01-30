using DG.Tweening;
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
        private float _startTime;

        private bool _isFaceWall = false;
        
        [SerializeField] private Transform _transform;
        
        private void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer("SickHuman");
            triggerType = TriggerEventType.Infect;
            _startTime = int.MinValue;
        }

        private void FixedUpdate()
        {
            var impactTime = Time.time;
            if (impactTime - _startTime < MoveDeltaTime)
            {
                return;
            }

            _direction = GetDirection();
            _startTime = impactTime;
            _transform.DOMove(_transform.position + (Vector3)HumanDirectionExtension.DirectionMap[_direction], MoveDeltaTime);
        }

        private HumanDirectionType GetDirection()
        {
            if (_isFaceWall)
            {
                _isFaceWall = false;
                return HumanDirectionExtension.GetReverseDirection(_direction);
            }

            return (HumanDirectionType)Random.Range(0, 7);
        }

        public void Back()
        {
            _isFaceWall = true;
        }
    }
}
