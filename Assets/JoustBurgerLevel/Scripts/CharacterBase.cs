using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterBase : MonoBehaviour
{
    public float move_speed = 6f;
    public float jump_force = 14f;
    public float invulnerable_time = 0.8f;

    public Transform ground_check_origin;
    public float ground_check_radius = 0.2f;
    public LayerMask ground_layer;

    protected Rigidbody2D rigidbody;
    protected float horizontal_move;
    public bool is_grounded;
    protected float invulnerable_until;

    public event Action on_ledge_fall;
    public event Action<CharacterBase> on_died;

    public Transform burger_layer_origin;
    public List<BurgerLayerSO> layers = new List<BurgerLayerSO>();
    protected List<GameObject> active_layers = new List<GameObject>();
    public GameObject BurgerLayerPrefab;

    public int LayerCount {
        get { return active_layers.Count; }
    }

    public bool IsAlive {
        get { return active_layers.Count > 0; }
    }

    protected virtual void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        if (rigidbody != null) {
            rigidbody.gravityScale = 2;
        }
    }

    protected virtual void Start() {
        foreach (BurgerLayerSO layer in layers) {
            InstantiateLayer(layer);
        }
    }

    protected void Update() {
        CheckGrounded();

        if (rigidbody != null) {
            rigidbody.linearVelocityX = move_speed * horizontal_move;
        }
    }


    public void Init(List<BurgerLayerSO> layers) {
        foreach (BurgerLayerSO layer in layers) {
            this.layers.Add(layer);
        }
    }

    public void AddLayer(BurgerLayerSO layer) {
        if (layer != null) {
            InstantiateLayer(layer);
        }
    }

    public void InstantiateLayer(BurgerLayerSO layer) {

        if (layer == null || BurgerLayerPrefab == null || burger_layer_origin == null) {
            Debug.LogWarning("Missing layer data, prefab, or origin on " + name, this);
            return;
        }

        GameObject newLayer = Instantiate(BurgerLayerPrefab, transform);

        BurgerLayerHurtbox hurtbox = newLayer.GetComponent<BurgerLayerHurtbox>();
        if (hurtbox == null) {
            hurtbox = newLayer.AddComponent<BurgerLayerHurtbox>();
        }

        hurtbox.Setup(this, layer);

        SpriteRenderer spriteRenderer = newLayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) {
            spriteRenderer.sprite = layer.sprite;
        }

        active_layers.Add(newLayer);
        RestackLayers();
    }

    public bool TryRemoveTopLayer(out BurgerLayerSO removedLayer) {
        removedLayer = null;

        if (active_layers.Count == 0) {
            return false;
        }

        int topIndex = active_layers.Count - 1;
        GameObject topLayer = active_layers[topIndex];

        BurgerLayerHurtbox hurtbox = topLayer.GetComponent<BurgerLayerHurtbox>();
        if (hurtbox != null) {
            removedLayer = hurtbox.layer;
        }

        active_layers.RemoveAt(topIndex);
        Destroy(topLayer);
        RestackLayers();

        if (active_layers.Count == 0 && on_died != null) {
            on_died(this);
        }

        return true;
    }

    public bool TakeHit(CharacterBase source) {
        if (Time.time < invulnerable_until) {
            return false;
        }

        invulnerable_until = Time.time + invulnerable_time;

        bool lostLayer = TryRemoveTopLayer(out _);

        if (lostLayer) {
            string attacker = source != null ? source.name : "a hit";
            Debug.Log(name + " lost a layer from " + attacker + ". Layers left: " + LayerCount, this);
        }

        return lostLayer;
    }

    public void GrantInvulnerability(float seconds) {
        invulnerable_until = Mathf.Max(invulnerable_until, Time.time + seconds);
    }

    private void RestackLayers() {
        if (burger_layer_origin == null) {
            return;
        }

        float nextY = burger_layer_origin.localPosition.y;

        foreach (GameObject activeLayer in active_layers) {
            BoxCollider2D layerCollider = activeLayer.GetComponent<BoxCollider2D>();
            float layerHeight = layerCollider != null ? layerCollider.size.y : 0.25f;

            nextY += layerHeight * 0.5f;
            activeLayer.transform.localPosition = new Vector3(burger_layer_origin.localPosition.x, nextY, 0f);
            nextY += layerHeight * 0.5f;
        }
    }

    public void CheckGrounded() {
        if (ground_check_origin == null) {
            return;
        }

        bool was_grounded = is_grounded;
        is_grounded = Physics2D.OverlapCircle(ground_check_origin.position, ground_check_radius, ground_layer);
        if (was_grounded && !is_grounded) {
            if (on_ledge_fall != null) {
                on_ledge_fall();
            }
        }
    }

    protected virtual void Jump() {
        if (is_grounded) {
            rigidbody.linearVelocityY = jump_force;
        }
    }
}
