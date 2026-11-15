using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float move_speed = 2f;
    public float jump_force = 4f;
    public Transform ground_check_origin;
    public float ground_check_radius = 1f;
    public LayerMask ground_layer;

    private Rigidbody2D rigidbody;
    private float horizontal_move;
    private bool is_grounded;

    private PlayerInputActions actions;

    void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        actions = new PlayerInputActions();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
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


    // Update is called once per frame
    void Update() {
        is_grounded = Physics2D.OverlapCircle(ground_check_origin.position, ground_check_radius, ground_layer);
        rigidbody.linearVelocityX = move_speed * horizontal_move;
    }

    void Move(InputAction.CallbackContext ctx) {
        print("HIIII!!!!");
        horizontal_move = ctx.ReadValue<Vector2>().x;
    }

    void Jump(InputAction.CallbackContext ctx) {
        if (is_grounded) 
            rigidbody.linearVelocityY = jump_force;
    }

}
