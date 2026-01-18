using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InteractableUI : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerClickHandler
{
    [Header("Scale")]
    public float hoverScale = 1.1f;
    public float pressScale = 0.95f;
    public float scaleSpeed = 12f;

    [Header("Colors")]
    public Color hoverColor = Color.yellow;
    public Color pressColor = Color.green;
    public Color tappedColor = Color.red;

    private Vector3 _baseScale;
    private Graphic _graphic;
    private Color _baseColor;

    private bool _hovered;
    private bool _pressed;
    private bool _tapped;

    private void Awake()
    {
        _baseScale = transform.localScale;
        _graphic = GetComponent<Graphic>();

        if (_graphic != null)
            _baseColor = _graphic.color;
    }

    private void Update()
    {
        UpdateScale();
        UpdateColor();
    }

    private void UpdateScale()
    {
        Vector3 targetScale = _baseScale;

        if (_pressed)
            targetScale = _baseScale * pressScale;
        else if (_hovered)
            targetScale = _baseScale * hoverScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.unscaledDeltaTime * scaleSpeed
        );
    }

    private void UpdateColor()
    {
        if (_graphic == null)
            return;

        if (_pressed)
            _graphic.color = pressColor;
        else if (_hovered)
            _graphic.color = hoverColor;
        else if (_tapped)
            _graphic.color = tappedColor;
        else
            _graphic.color = _baseColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        _hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        _pressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _pressed = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _tapped = !_tapped;
    }
}
