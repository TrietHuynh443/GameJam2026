using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    public InputActionAsset InputActions;
    public float walkSpeed = 5f;

    private InputAction _move;

    private void Awake()
    {
        _move = InputActions.FindActionMap("Player").FindAction("Move");
    }

    private void OnEnable()
    {
        _move.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
    }

    private void Update()
    {
        Vector2 input = _move.ReadValue<Vector2>();
        transform.position += (Vector3)(input * walkSpeed * Time.deltaTime);
    }
}