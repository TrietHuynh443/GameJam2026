using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameEvent.Events;
using Sound;
using UnityEditor.Animations;
using UnityEngine;

namespace Human
{
    public enum HumanState
    {
        Normal,
        Angry,
        Sick
    }
    
    public enum BubbleState
    {
        Masked,
        Angry,
        Sick
    }

    public class NPCStateController : MonoBehaviour
    {
        public bool isMasked = false;
        public bool isBeingDragged = false;
        private Transform _dragSource;

        [Header("State Objects")]
        [SerializeField] private GameObject normal;
        [SerializeField] private GameObject angry;
        [SerializeField] private GameObject sick;

        [Header("Sprite")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Skins")]
        [SerializeField] private List<AnimatorController> skinControllers;

        [Header("State Sprites")]
        [SerializeField] private Sprite normalSprite; // Tilesets_91
        [SerializeField] private Sprite angrySprite;  // Tilesets_98
        [SerializeField] private Sprite sickSprite;   // Tilesets_88
        [SerializeField] private SickHuman _sickHuman;
        [SerializeField] private HumanAngry _angryHuman;
        [SerializeField] private HumanNormal _normalHuman;

        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _animatorMask;
        [SerializeField] public HumanState currentState = HumanState.Normal;
        

        [SerializeField] private GameObject _bubble;
        [SerializeField] private GameObject _angryIcon;
        [SerializeField] private GameObject _maskedIcon;
        [SerializeField] private GameObject _sickIcon;
        
        [SerializeField] private GameObject _feverObject;
        private IHuman _current;
        private void OnEnable()
        {
            if (_current == null)
            {
                SetState(currentState);
            }
            GameEvent.GameEvent.Subscribe<InfectedEvent>(OnInfected);
            GameEvent.GameEvent.Subscribe<EntityMaskedEvent>(OnMasked);
            GameEvent.GameEvent.Subscribe<EntityDragEvent>(OnDrag);
            GameEvent.GameEvent.Subscribe<EntityStopDragEvent>(OnStopDrag);
        }
        
        private void Awake()
        {
            if (!_animator) return;
            if (skinControllers == null || skinControllers.Count == 0)
            {
                Debug.LogWarning("No skin controllers assigned.");
                return;
            }

            int randomIndex = Random.Range(0, skinControllers.Count);
            _animator.runtimeAnimatorController = skinControllers[randomIndex];
        }


        private void Start()
        {
            SetState(currentState);
        }

        private void OnMasked(EntityMaskedEvent evt)
        {
            if (evt.HumanNormal.transform.parent?.gameObject != gameObject || currentState == HumanState.Sick)
                return;

            if (currentState is HumanState.Angry)
                _angryHuman.Fight(evt);
            else
            {
                _normalHuman.Masked();
                _angryHuman.Masked();
                SetBubble(BubbleState.Masked);
                SoundManager.Instance.PlaySoundEffect(SoundEffectType.Masked);
                StartCoroutine(WaitAndTurn());
            }
        }


        private void OnInfected(InfectedEvent obj)
        {
            if(obj.Human.transform.parent?.gameObject != gameObject) 
                return;
            
            _normalHuman.Infected();
            _angryHuman.Infected();
            SetBubble(BubbleState.Sick);
            
            StartCoroutine(WaitAndTurn());
        }

        private void OnDrag(EntityDragEvent evt)
        {
            // Only react if THIS NPC is the target
            if (evt.Target.transform.parent?.gameObject != gameObject)
                return;

            isBeingDragged = true;
            _dragSource = evt.Source.transform;
        }

        private void OnStopDrag(EntityStopDragEvent evt)
        {
            if (evt.Target.transform.parent?.gameObject != gameObject)
                return;

            isBeingDragged = false;
            _dragSource = null;
        }


        private void SetBubble(BubbleState state)
        {
            _angryIcon.SetActive(state == BubbleState.Angry);
            _maskedIcon.SetActive(state == BubbleState.Masked);
            _sickIcon.SetActive(state == BubbleState.Sick);
            _bubble.SetActive(true);
            UniTask.WaitForSeconds(1f).ContinueWith(() => _bubble.SetActive(false));
        }
        
        

        private void OnDisable()
        {
            GameEvent.GameEvent.Unsubscribe<InfectedEvent>(OnInfected);
            GameEvent.GameEvent.Unsubscribe<EntityMaskedEvent>(OnMasked);
            GameEvent.GameEvent.Unsubscribe<EntityDragEvent>(OnDrag);
            GameEvent.GameEvent.Unsubscribe<EntityStopDragEvent>(OnStopDrag);

        }

        public void SetState(HumanState state)
        {
            currentState = state;
            switch (state)
            {
                case HumanState.Normal:
                    _current = _normalHuman;
                    break;

                case HumanState.Angry:
                    _current = _angryHuman;
                    SetBubble(BubbleState.Angry);
                    break;

                case HumanState.Sick:
                    _current = _sickHuman;
                    break;
            }

            normal.SetActive(state == HumanState.Normal);
            angry.SetActive(state == HumanState.Angry);
            sick.SetActive(state == HumanState.Sick);
        }

        private bool _isWaiting = false;
        [SerializeField] private GameObject _maskedObject;

        private void FixedUpdate()
        {
            _maskedObject.SetActive(isMasked);
            _feverObject.SetActive(currentState is HumanState.Sick);
            if (isBeingDragged && _dragSource != null)
            {
                // Follow player
                transform.position = Vector3.Lerp(
                    transform.position,
                    _dragSource.position,
                    15f * Time.fixedDeltaTime
                );
                return;
            }

            if (_isWaiting)
            {
                _current.RotateAround();
                return;
            }

            _current.Move();
        }


        public bool CheckObstacle(Vector2 dir)
        {
            int layerIndex = LayerMask.NameToLayer("Obstacle");
            int layerMask = 1 << layerIndex;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, 2f, layerMask);
            if (hit.collider != null)
            {
                StartCoroutine(WaitAndTurn());
                return true;
            }

            return false;
        }

        private IEnumerator WaitAndTurn()
        {
            if (_isWaiting) yield break;
            
            float speed = _animator.speed;
            _isWaiting = true;
            _animator.speed = 0;
            _animatorMask.speed = 0;
            yield return new WaitForSeconds(1f);
            _animator.speed = speed;
            _animatorMask.speed = speed;
            _isWaiting = false;
        }

        public void ResetState()
        {
            SetState(HumanState.Normal);
        }
    }
}