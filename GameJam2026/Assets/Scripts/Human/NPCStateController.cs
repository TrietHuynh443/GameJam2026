using UnityEngine;

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

        public void SetState(HumanState state)
        {
            normal.SetActive(state == HumanState.Normal);
            angry.SetActive(state == HumanState.Angry);
            sick.SetActive(state == HumanState.Sick);

            switch (state)
            {
                case HumanState.Normal:
                    spriteRenderer.sprite = normalSprite;
                    break;

                case HumanState.Angry:
                    spriteRenderer.sprite = angrySprite;
                    break;

                case HumanState.Sick:
                    spriteRenderer.sprite = sickSprite;
                    break;
            }
        }

        public void ResetState()
        {
            SetState(HumanState.Normal);
        }
    }
}