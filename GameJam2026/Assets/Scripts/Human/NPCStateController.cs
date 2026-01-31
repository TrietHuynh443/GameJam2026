using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameEvent.Events;
using Sound;
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
        Sick,
        Healing,
        Dead
    }

    public class NPCStateController : MonoBehaviour
    {
        public int isMasked = 0;
        public bool isBeingDragged = false;
        private Transform _dragSource;

        [Header("State Objects")]
        [SerializeField] private GameObject normal;
        [SerializeField] private GameObject angry;
        [SerializeField] private GameObject sick;

        [Header("Sprite")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        [Header("Skins")]
        [SerializeField] private List<RuntimeAnimatorController> skinControllers;

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
        [SerializeField] private GameObject _healIcon;
        [SerializeField] private GameObject _deadIcon;
        
        [SerializeField] private GameObject _auraObject;
        [SerializeField] private GameObject _angryObject;
        
        private bool _isWaiting = false;
        [SerializeField] private GameObject _maskedObject;
        
        [Header("After Cure Movement")]
        [SerializeField] private List<Vector2> curedDestinations;
        [SerializeField] private float moveToCuredSpeed = 2.5f;
        [SerializeField] private float fadeOutDuration = 1f;

        private Coroutine _moveAfterCureRoutine;
        private Coroutine _fadeOutRoutine;
        private bool _isMovingAfterCure;
        
        [Header("Dead Timer")]
        [SerializeField] private float deadDuration = 45f;

        private float _deadTimer;
        private bool _isDeadCounting;
        private SpriteRenderer _auraRenderer;
        private readonly Color _feverColor = new Color32(119, 106, 162, 147);

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
            GameEvent.GameEvent.Subscribe<EntityDeadEvent>(OnDead);
        }
        
        private void Awake()
        {
            if (!_animator) return;
            
            if (_auraObject != null)
                _auraRenderer = _auraObject.GetComponent<SpriteRenderer>();
            
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
                || isMasked > 0)
                return;

            if (currentState is HumanState.Angry)
                _angryHuman.Fight(evt);
            else
            {
                isMasked += 25;
                SetBubble(BubbleState.Masked);
                SoundManager.Instance.PlaySoundEffect(SoundEffectType.Masked);
                GameEvent.GameEvent.Publish(new ScoreEvent(0, 1, 0));
                StartCoroutine(WaitAndTurn());
            }
        }


        private void OnInfected(InfectedEvent obj)
        {
            if(obj.Human.transform.parent?.gameObject != gameObject || _current is SickHuman) 
                return;

            if (isMasked > 0)
            {
                isMasked -= 1;
                if (isMasked == 0) 
                    GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent(0, -1, 0));
            }
            else
            {
                GameEvent.GameEvent.Publish<ScoreEvent>(new ScoreEvent(1, 0, -1));
                SetState(HumanState.Sick);
                StartCoroutine(WaitAndTurn());
                SetBubble(BubbleState.Sick);
            }
            

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

            _sickHuman.Cured();

            StartCoroutine(WaitTillRelease());
        }

        private void OnDead(EntityDeadEvent evt)
        {
            if (evt.Target.transform.parent?.gameObject != gameObject)
                return;
            SetBubble(BubbleState.Dead, 1f);

            StartCoroutine(FadeOutAndDisable());
        }



        public void SetBubble(BubbleState state, float duration = 1f)
        {
            _angryIcon.SetActive(state == BubbleState.Angry);
            _maskedIcon.SetActive(state == BubbleState.Masked);
            _sickIcon.SetActive(state == BubbleState.Sick);
            _healIcon.SetActive(state == BubbleState.Healing);
            _deadIcon.SetActive(state == BubbleState.Dead);
            _bubble.SetActive(true);
            UniTask.WaitForSeconds(duration).ContinueWith(() => _bubble.SetActive(false));
        }
        
        

        private void OnDisable()
        {
            GameEvent.GameEvent.Unsubscribe<InfectedEvent>(OnInfected);
            GameEvent.GameEvent.Unsubscribe<EntityMaskedEvent>(OnMasked);
            GameEvent.GameEvent.Unsubscribe<EntityDragEvent>(OnDrag);
            GameEvent.GameEvent.Unsubscribe<EntityStopDragEvent>(OnStopDrag);
            GameEvent.GameEvent.Unsubscribe<EntityCureEvent>(OnCure);
            GameEvent.GameEvent.Unsubscribe<EntityDeadEvent>(OnDead);

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
            float startAlphaAura = _auraRenderer.color.a;

            while (time < fadeOutDuration)
            {
                time += Time.deltaTime;
                float t = time / fadeOutDuration;
                SetAlpha(Mathf.Lerp(startAlpha, 0f, t));
                SetAlphaAura(Mathf.Lerp(startAlphaAura, 0f, t));
                yield return null;
            }

            SetAlpha(0f);
            SetAlphaAura(0f);
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
        
        private void SetAlphaAura(float alpha)
        {
            if (_auraRenderer == null) return;

            Color c = _auraRenderer.color;
            c.a = alpha;
            _auraRenderer.color = c;
        }

        public void SetState(HumanState state)
        {
            currentState = state;
            switch (state)
            {
                case HumanState.Normal:
                    _current = _normalHuman;
                    _isDeadCounting = false;
                    break;



                case HumanState.Angry:
                    _current = _angryHuman;
                    SetBubble(BubbleState.Angry);
                    break;

                case HumanState.Sick:
                    _current = _sickHuman;

                    if (_auraRenderer != null)
                        _auraRenderer.color = _feverColor;

                    _deadTimer = 0f;
                    _isDeadCounting = true;
                    break;
            }

            normal.SetActive(state == HumanState.Normal);
            angry.SetActive(state == HumanState.Angry);
            sick.SetActive(state == HumanState.Sick);
        }
        
        private void Update()
        {
            if (!_isDeadCounting || currentState != HumanState.Sick)
                return;

            _deadTimer += Time.deltaTime;

            float t = Mathf.Clamp01(_deadTimer / deadDuration);

            // Lerp fever color → black
            if (_auraRenderer != null)
                _auraRenderer.color = Color.Lerp(_feverColor, Color.black, t);

            if (_deadTimer >= deadDuration)
            {
                _isDeadCounting = false;
                _sickHuman.Dead();
            }
        }


        private void FixedUpdate()
        {
            _maskedObject.SetActive(isMasked > 0);
            _auraObject.SetActive(currentState is HumanState.Sick);
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

        private IEnumerator WaitTillRelease()
        {
            yield return new WaitUntil(() => isBeingDragged == false);

            if (_isMovingAfterCure)
                yield break;

            StartCoroutine(MoveAfterCure());
        }

        private Vector2 GetNearestCuredDestination()
        {
            Vector2 best = new Vector2(100f, 100f);
            float minDist = float.MaxValue;

            foreach (var t in curedDestinations)
            {
                float d = Vector2.Distance(transform.position, t);
                if (d < minDist)
                {
                    minDist = d;
                    best = t;
                }
            }
            return best;
        }

        
        private IEnumerator MoveAfterCure()
        {
            Vector2 destination = GetNearestCuredDestination();
            SetBubble(BubbleState.Healing, 3f);
            yield return new WaitForSeconds(3f);

            _isMovingAfterCure = true;

            SetState(HumanState.Normal);

            while (Vector2.Distance(transform.position, destination) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    destination,
                    moveToCuredSpeed * Time.deltaTime
                );
                

                yield return null;
            }
            _isMovingAfterCure = false;

        }


        public void ResetState()
        {
            SetState(HumanState.Normal);
        }
    }
}