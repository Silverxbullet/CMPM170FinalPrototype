using UnityEngine;
using System;
using System.Collections.Generic;

public class RunningEnemy : CharacterBase
{
    protected override void Awake() {
        base.Awake();
        horizontal_move = 1f;
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
