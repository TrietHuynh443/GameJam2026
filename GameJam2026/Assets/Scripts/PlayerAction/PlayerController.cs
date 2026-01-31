using UnityEngine;
using UnityEngine.InputSystem;

namespace Human.PlayerAction
{
    public class PlayerController: MonoBehaviour
    {
        public InputActionAsset InputActions;
        public float walkSpeed = 5f;
        public float sprintMultiplier = 1.6f;

        private InputAction _move;
        private InputAction _sprint;
        private Animator _animator;

        private HumanDirectionType _currentDir = HumanDirectionType.Bottom;
        private bool _wasMoving;

        private void Awake()
        {
            var playerMap = InputActions.FindActionMap("Player");
            _move = playerMap.FindAction("Move");
            _sprint = playerMap.FindAction("Sprint");

            _animator = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            _move.Enable();
            _sprint.Enable();
        }

        private void OnDisable()
        {
            _move.Disable();
            _sprint.Disable();
        }

        private void Update()
        {
            Vector2 input = _move.ReadValue<Vector2>();
            bool isMoving = input.sqrMagnitude > 0.001f;

            bool isSprinting = _sprint.IsPressed();
            float speed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;

            Vector2 moveDir = input.normalized;
            transform.position += (Vector3)(moveDir * (speed * Time.deltaTime));

            if (isMoving)
            {
                HumanDirectionType newDir = HumanDirectionExtension.GetDirection(moveDir);
                
                PlayAnimation(newDir);
                _currentDir = newDir;
            }
            else if (_wasMoving)
            {
                PlayAnimation(_currentDir);
            }

            _wasMoving = isMoving;
        }
        
        private void PlayAnimation(string animationName)
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            {
                _animator.Play(animationName);
            }
        }
        
        private void PlayAnimation(HumanDirectionType direction)
        {
            string animationName = HumanDirectionExtension.DirectionNameMap[direction];
            PlayAnimation(animationName);
        }
    }
}