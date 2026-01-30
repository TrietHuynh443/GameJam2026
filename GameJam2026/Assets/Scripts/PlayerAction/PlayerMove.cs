using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    public InputActionAsset InputActions;

    public float walkSpeed = 5f;
    public float sprintMultiplier = 1.6f;

    private InputAction _move;
    private InputAction _sprint;

    private void Awake()
    {
        var playerMap = InputActions.FindActionMap("Player");

        _move = playerMap.FindAction("Move");
        _sprint = playerMap.FindAction("Sprint");
    }

    private void OnEnable()
    {
        _move.Enable();
        _sprint.Enable();
    }

    private void OnDisable()
    {
        _move.Disable();
        _sprint.Disable();
    }

    private void Update()
    {
        Vector2 input = _move.ReadValue<Vector2>();

        bool isSprinting = _sprint.IsPressed();
        float speed = isSprinting ? walkSpeed * sprintMultiplier : walkSpeed;

        transform.position += (Vector3)(input * speed * Time.deltaTime);
    }
}