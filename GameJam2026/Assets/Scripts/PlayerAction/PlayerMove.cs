using Trigger;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerWalk : TriggerObject
{
    public InputActionAsset InputActions;

    public float walkSpeed = 5f;
    public float sprintMultiplier = 1.6f;

    private InputAction _move;
    private InputAction _sprint;
    private InputAction _giveMask;
    
    [SerializeField] private Image _image;
    
    private bool _interactPressedThisFrame;
    private int _triggeredLayer;

    protected override void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");

        _move = playerMap.FindAction("Move");
        _sprint = playerMap.FindAction("Sprint");
        _giveMask = playerMap.FindAction("Interact");
        
        _triggeredLayer = LayerMask.NameToLayer("Human");
        base.Awake();

    }

    private void OnEnable()
    {
        _move.Enable();
        _sprint.Enable();
        _giveMask.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _sprint.Disable();
        _giveMask.Disable();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if(_triggeredLayer.Equals(other.gameObject.layer))
            _image.gameObject.SetActive(true);
        base.OnTriggerEnter2D(other);
    }
    
    protected override void OnTriggerStay2D(Collider2D other)
    {
        if (_giveMask.WasPressedThisFrame())
            base.OnTriggerStay2D(other);
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        if(_triggeredLayer.Equals(other.gameObject.layer))
            _image.gameObject.SetActive(false);
        base.OnTriggerExit2D(other);
    }

    private void FixedUpdate()
    {
        Vector2 input = _move.ReadValue<Vector2>();

        bool isSprinting = _sprint.IsPressed();
        float speed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;

        transform.position += (Vector3)(input * speed * Time.fixedDeltaTime);
    }

}