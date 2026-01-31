using UnityEngine;
using UnityEngine.InputSystem;
using Trigger;
using Human;

namespace PlayerAction
{
    public class PlayerController: TriggerObject
    {
        public InputActionAsset InputActions;
        public float walkSpeed = 5f;
        public float sprintMultiplier = 1.6f;

        private InputAction _move;
        private InputAction _sprint;
        private InputAction _applyMask;
        private InputAction _drag;
        private Animator _animator;

        private HumanDirectionType _currentDir = HumanDirectionType.Bottom;
        private bool _wasMoving;
        private bool _isDragging = false;
        private Collider2D _dragTarget;
        private float _nextDragAllowedTime;

        [SerializeField] private float dragToggleCooldown = 0.25f;
        
        private void Awake()
        {
            var playerMap = InputActions.FindActionMap("Player");
            _move = playerMap.FindAction("Move");
            _sprint = playerMap.FindAction("Sprint");
            _applyMask = playerMap.FindAction("Interact");
            _drag = playerMap.FindAction("Drag");
            

            _animator = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            _move.Enable();
            _sprint.Enable();
            _applyMask.Enable();
            _drag.Enable();
            isAuto = false;
            
            base.OnEnable();
        }

        private void OnDisable()
        {
            _move.Disable();
            _sprint.Disable();
            _applyMask.Disable();
            _drag.Disable();
        }

        protected override void OnTriggerStay2D(Collider2D other)
        {
            // 1. Independent action (Masking)
            if (_applyMask.IsPressed())
            {
                ExecuteActions(other, TriggerPhase.Stay, TriggerEventType.ApplyMask);
            }

            // 2. Guard Clauses for Dragging
            if (!_drag.IsPressed()) return;
            if (Time.time < _nextDragAllowedTime) return;

            // 3. Logic Branching
            if (!_isDragging)
            {
                // START DRAGGING
                _isDragging = true;
                _dragTarget = other;
                ExecuteActions(_dragTarget, TriggerPhase.Stay, TriggerEventType.Drag);
        
                // Apply cooldown after starting
                _nextDragAllowedTime = Time.time + dragToggleCooldown;
            }
            else if (_dragTarget == other)
            {
                // STOP DRAGGING
                ExecuteActions(_dragTarget, TriggerPhase.Stay, TriggerEventType.StopDrag);
                _isDragging = false;
                _dragTarget = null;

                // Apply cooldown after stopping
                _nextDragAllowedTime = Time.time + dragToggleCooldown;
            }
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