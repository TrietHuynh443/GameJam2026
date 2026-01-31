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
        public bool isImmune = false;
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
        [SerializeField] private GameObject _angryObject;
        
        private bool _isWaiting = false;
        [SerializeField] private GameObject _maskedObject;
        
        [Header("After Cure Movement")]
        [SerializeField] private Transform[] curedDestinations;
        [SerializeField] private float moveToCuredSpeed = 2.5f;
        [SerializeField] private float fadeOutDuration = 0.5f;

        private Coroutine _moveAfterCureRoutine;
        private Coroutine _fadeOutRoutine;
        private bool _isMovingAfterCure;

        
        [Header("Fade In")]
        [SerializeField] private float fadeInDuration = 0.5f;
        private Coroutine _fadeRoutine;
        
        private IHuman _current;
        private void OnEnable()
        {
            SetAlpha(0f);

            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);

            _fadeRoutine = StartCoroutine(FadeIn());
            
            if (_current == null)
            {
                SetState(currentState);
            }
            GameEvent.GameEvent.Subscribe<InfectedEvent>(OnInfected);
            GameEvent.GameEvent.Subscribe<EntityMaskedEvent>(OnMasked);
            GameEvent.GameEvent.Subscribe<EntityDragEvent>(OnDrag);
            GameEvent.GameEvent.Subscribe<EntityStopDragEvent>(OnStopDrag);
            GameEvent.GameEvent.Subscribe<EntityCureEvent>(OnCure);
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
            if (evt.HumanNormal.transform.parent?.gameObject != gameObject 
                || currentState == HumanState.Sick
                || isMasked)
                return;

            if (currentState is HumanState.Angry)
                _angryHuman.Fight(evt);
            else
            {
                isMasked = true;
                SetBubble(BubbleState.Masked);
                SoundManager.Instance.PlaySoundEffect(SoundEffectType.Masked);
                GameEvent.GameEvent.Publish(new ScoreEvent(0, 1, 0));
                StartCoroutine(WaitAndTurn());
            }
        }


        private void OnInfected(InfectedEvent obj)
        {
            if(obj.Human.transform.parent?.gameObject != gameObject || _current is SickHuman || isImmune) 
                return;
            
            if (isMasked)
            {
                isMasked = false;
                GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent(0, -1, 0));
            }
            else
            {
                GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent(1, 0, 0));
            }
            
            SetState(HumanState.Sick);
            
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
        
        private void OnCure(EntityCureEvent evt)
        {
            if (evt.Target.transform.parent?.gameObject != gameObject)
                return;

            if (_isMovingAfterCure)
                return;

            isImmune = true;
            _sickHuman.Cured();

            isBeingDragged = false;
            _dragSource = null;

            StartCoroutine(WaitAndTurn());

            _moveAfterCureRoutine = StartCoroutine(MoveAfterCure());
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
            GameEvent.GameEvent.Unsubscribe<EntityCureEvent>(OnCure);

        }
        
        private IEnumerator FadeIn()
        {
            float time = 0f;

            while (time < fadeInDuration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Clamp01(time / fadeInDuration);
                SetAlpha(alpha);
                yield return null;
            }

            SetAlpha(1f);
        }
        
        private IEnumerator FadeOutAndDisable()
        {
            if (_fadeOutRoutine != null)
                StopCoroutine(_fadeOutRoutine);

            float time = 0f;
            float startAlpha = spriteRenderer.color.a;

            while (time < fadeOutDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeOutDuration;
                SetAlpha(Mathf.Lerp(startAlpha, 0f, t));
                yield return null;
            }

            SetAlpha(0f);
            _isMovingAfterCure = false;

            gameObject.SetActive(false); // returned to pool AFTER fade
        }


        private void SetAlpha(float alpha)
        {
            if (spriteRenderer == null) return;

            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
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


        private void FixedUpdate()
        {
            _maskedObject.SetActive(isMasked);
            _feverObject.SetActive(currentState is HumanState.Sick);
            _angryObject.SetActive(currentState is HumanState.Angry);

            if (_isMovingAfterCure)
                return;

            if (isBeingDragged && _dragSource != null)
            {
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
        
        private Transform GetRandomCuredDestination()
        {
            if (curedDestinations == null || curedDestinations.Length == 0)
                return null;

            return curedDestinations[Random.Range(0, curedDestinations.Length)];
        }

        
        private IEnumerator MoveAfterCure()
        {
            Transform destination = GetRandomCuredDestination();
            if (destination == null)
            {
                Debug.LogWarning("No cured destinations assigned", this);
                yield break;
            }

            _isMovingAfterCure = true;

            SetState(HumanState.Normal);

            while (Vector2.Distance(transform.position, destination.position) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    destination.position,
                    moveToCuredSpeed * Time.deltaTime
                );

                yield return null;
            }

            yield return FadeOutAndDisable();
        }

        public void ResetState()
        {
            SetState(HumanState.Normal);
        }
    }
}