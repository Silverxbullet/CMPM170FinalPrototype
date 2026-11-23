using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterBase
{
    [Header("Attack")]
    public Transform attack_point;
    public Vector2 attack_size = new Vector2(1.6f, 1.2f);
    public float attack_cooldown = 0.35f;
    public float attack_invulnerability = 0.15f;
    public GameObject attack_visual;

    private PlayerInputActions actions;
    private float next_attack_time;

    protected override void Awake() {
        base.Awake();
        actions = new PlayerInputActions();
        on_died += HandleDied;
    }

    protected override void Start() {
        base.Start();
        actions.Player.Enable();
    }

    private void OnEnable() {
        actions.Player.Move.performed += Move;
        actions.Player.Move.canceled += Move;
        actions.Player.Jump.performed += Jump;
        actions.Player.Fire.performed += Attack;
    }

    private void OnDisable() {
        actions.Player.Move.performed -= Move;
        actions.Player.Move.canceled -= Move;
        actions.Player.Jump.performed -= Jump;
        actions.Player.Fire.performed -= Attack;

        actions.Player.Disable();
    }

    private void OnDestroy() {
        if (actions != null) {
            actions.Dispose();
        }
    }

    private void Move(InputAction.CallbackContext context) {
        horizontal_move = context.ReadValue<Vector2>().x;
    }

    private void Jump(InputAction.CallbackContext context) {
        base.Jump();
    }

    private void Attack(InputAction.CallbackContext context) {

        if (!IsAlive) {
            return;
        }

        if (Time.time < next_attack_time) {
            return;
        }

        next_attack_time = Time.time + attack_cooldown;

        if (attack_visual != null) {
            StartCoroutine(ShowAttackVisual());
        }

        // Brief protection prevents trading damage in the same frame.
        GrantInvulnerability(attack_invulnerability);

        PerformAttack();
    }

    private IEnumerator ShowAttackVisual() {
        attack_visual.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        attack_visual.SetActive(false);
    }

    private void PerformAttack() {
        
        Vector2 attackCenter = attack_point != null ? (Vector2)attack_point.position : (Vector2)transform.position;

        Collider2D[] hits = Physics2D.OverlapBoxAll(attackCenter, attack_size, 0f);

        HashSet<CharacterBase> alreadyHit = new HashSet<CharacterBase>();

        foreach (Collider2D hit in hits) {
            BurgerLayerHurtbox hurtbox = hit.GetComponent<BurgerLayerHurtbox>();

            if (hurtbox == null) {
                continue;
            }

            CharacterBase target = hurtbox.owner != null ? hurtbox.owner : hit.GetComponentInParent<CharacterBase>();

            if (target == null || target == this) {
                continue;
            }

            if (!(target is RunningEnemy)) {
                continue;
            }

            if (!alreadyHit.Add(target)) {
                continue;
            }

            if (target.TryRemoveTopLayer(out BurgerLayerSO stolenLayer)) {
                HitFlash hit_flash = target.GetComponent<HitFlash>();

                if (hit_flash != null) {
                    hit_flash.Flash();
                }

                AddLayer(stolenLayer);
                string layerName = stolenLayer != null ? stolenLayer.name : "a layer";
                Debug.Log("Stole " + layerName + ". Player layers: " + LayerCount, this);

                // One enemy per swing only
                break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        RunningEnemy enemy = collision.collider.GetComponentInParent<RunningEnemy>();

        if (enemy != null) {
            TakeHit(enemy);
        }
    }

    private void HandleDied(CharacterBase character) {
        horizontal_move = 0f;
        move_speed = 0f;

        if (rigidbody != null) {
            rigidbody.linearVelocity = Vector2.zero;
        }

        Debug.Log("Player has no burger layers left.", this);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = attack_point != null ? attack_point.position : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, attack_size);
    }
}
