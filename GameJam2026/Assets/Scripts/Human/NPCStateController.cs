using System;
using System.Collections;
using Cysharp.Threading.Tasks;
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
            _isWaiting = true;
            yield return new WaitForSeconds(1f);
            _isWaiting = false;
        }

        public void ResetState()
        {
            SetState(HumanState.Normal);
        }
    }
}