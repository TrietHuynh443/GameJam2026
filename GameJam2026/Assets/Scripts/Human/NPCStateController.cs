using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameEvent.Events;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [Header("State Objects")]
        [SerializeField] private GameObject normal;
        [SerializeField] private GameObject angry;
        [SerializeField] private GameObject sick;

        [Header("Sprite")]
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Header("State Sprites")]
        [SerializeField] private Sprite normalSprite; // Tilesets_91
        [SerializeField] private Sprite angrySprite;  // Tilesets_98
        [SerializeField] private Sprite sickSprite;   // Tilesets_88
        [SerializeField] private SickHuman _sickHuman;
        [SerializeField] private HumanAngry _angryHuman;
        [SerializeField] private HumanNormal _normalHuman;

        [SerializeField] private HumanState _initState = HumanState.Normal;
        [SerializeField] private Animator _animator;
        

        [SerializeField] private GameObject _bubble;
        [SerializeField] private GameObject _angryIcon;
        [SerializeField] private GameObject _maskedIcon;
        [SerializeField] private GameObject _sickIcon;
        private IHuman _current;
        private void OnEnable()
        {
            if (_current == null)
            {
                SetState(_initState);
            }
            GameEvent.GameEvent.Subscribe<InfectedEvent>(OnInfected);
            GameEvent.GameEvent.Subscribe<EntityMaskedEvent>(OnMasked);
        }


        private void Start()
        {
            SetState(HumanState.Sick);
        }

        private void OnMasked(EntityMaskedEvent obj)
        {
            _normalHuman.Masked();
            _angryHuman.Masked();
        }

        private void OnInfected(InfectedEvent obj)
        {
            if(obj.Human.transform.parent?.gameObject != gameObject) 
                return;
            
            _normalHuman.Infected();
            _angryHuman.Infected();
            SetBubble(BubbleState.Masked);
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
        }

        public void SetState(HumanState state)
        {
            switch (state)
            {
                case HumanState.Normal:
                    _normalHuman.isMasked = _angryHuman.isMasked;
                    _current = _normalHuman;
                    break;

                case HumanState.Angry:
                    _angryHuman.isMasked = _normalHuman.isMasked;
                    _current = _angryHuman;
                    SetBubble(BubbleState.Angry);
                    break;

                case HumanState.Sick:
                    SetBubble(BubbleState.Sick);
                    _current = _sickHuman;
                    break;
            }
            
            

            normal.SetActive(state == HumanState.Normal);
            angry.SetActive(state == HumanState.Angry);
            sick.SetActive(state == HumanState.Sick);
        }

        private bool _isWaiting = false;
        private void FixedUpdate()
        {
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
            float speed = _animator.speed; 
            _isWaiting = true;
            _animator.speed = 0;
            yield return new WaitForSeconds(1f);
            _animator.speed = speed;
            _isWaiting = false;
        }

        public void ResetState()
        {
            SetState(HumanState.Normal);
        }
    }
}