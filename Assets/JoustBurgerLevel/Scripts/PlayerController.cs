using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{

    private PlayerInputActions actions;

    void Awake() {
        base.Awake();
        actions = new PlayerInputActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        base.Start();
        actions.Player.Enable();
    }

    void OnEnable() {
        actions.Player.Move.performed += Move;
        actions.Player.Jump.performed += Jump;

        actions.Player.Move.canceled += Move;
    }

    void OnDisable() {
        actions.Player.Move.performed -= Move;
        actions.Player.Jump.performed -= Jump;
        actions.Player.Disable();
    }

    void Move(InputAction.CallbackContext ctx) {
        print("HIIII!!!!");
        horizontal_move = ctx.ReadValue<Vector2>().x;
    }

    void Jump(InputAction.CallbackContext ctx) {
        Jump();
    }

}
