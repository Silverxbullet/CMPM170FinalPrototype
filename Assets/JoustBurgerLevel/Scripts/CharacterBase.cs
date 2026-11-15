using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    public float move_speed = 6f;
    public float jump_force = 14f;

    public Transform ground_check_origin;
    public float ground_check_radius = 0.2f;
    public LayerMask ground_layer;

    protected Rigidbody2D rigidbody;
    protected float horizontal_move;
    public bool is_grounded;

    public event Action on_ledge_fall;

    public Transform burger_layer_origin;
    public List<BurgerLayerSO> layers = new List<BurgerLayerSO>();
    protected List<GameObject> active_layers = new List<GameObject>();
    public GameObject BurgerLayerPrefab;

    protected virtual void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.gravityScale = 2;
    }

    protected virtual void Start() {
        if (layers.Count > 0) {
            foreach (BurgerLayerSO layer in layers) {
                InstantiateLayer(layer);
            }
        }
    }

    protected void Update() {
        CheckGrounded();
        rigidbody.linearVelocityX = move_speed * horizontal_move;
    }


    public void Init(List<BurgerLayerSO> layers) {
        foreach (BurgerLayerSO layer in layers) {
            this.layers.Add(layer);
        }
    }

    public void InstantiateLayer(BurgerLayerSO layer) {
        GameObject new_layer_instance = Instantiate(BurgerLayerPrefab, gameObject.transform);
        active_layers.Add(new_layer_instance);

        float layers_height = 0f;
        foreach (GameObject active_layer in active_layers) {
            layers_height += active_layer.GetComponent<BoxCollider2D>().size.y;
        }

        Vector2 layer_position = new Vector2(burger_layer_origin.localPosition.x, burger_layer_origin.localPosition.y + layers_height);
        new_layer_instance.transform.localPosition = layer_position;
        new_layer_instance.GetComponent<SpriteRenderer>().sprite = layer.sprite;
    }


    public void CheckGrounded() {
        bool was_grounded = is_grounded;
        is_grounded = Physics2D.OverlapCircle(ground_check_origin.position, ground_check_radius, ground_layer);
        if (was_grounded && !is_grounded) {
            on_ledge_fall?.Invoke();
        }
    }

    protected virtual void Jump() {
        if (is_grounded)
            rigidbody.linearVelocityY = jump_force;
    }
}
