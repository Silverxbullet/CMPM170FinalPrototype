using UnityEngine;
using System;
using System.Collections.Generic;

public class RunningEnemy : CharacterBase
{
    protected override void Awake() {
        base.Awake();
        horizontal_move = 1f;
        on_died += HandleDied;
    }

    private void OnDestroy() {
        on_died -= HandleDied;
    }

    private void HandleDied(CharacterBase character) {
        Destroy(gameObject);
    }

    public void OnEnable() {
        base.on_ledge_fall += Jump;
    }

    public void OnDisable() {
        this.on_ledge_fall -= Jump;
    }

    protected override void Jump() {
        rigidbody.linearVelocityY = jump_force;
    }
}
