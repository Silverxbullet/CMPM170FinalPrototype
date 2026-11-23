using UnityEngine;

public class BurgerLayerHurtbox : MonoBehaviour
{
    public CharacterBase owner;
    public BurgerLayerSO layer;

    private void Awake() {
        if (owner == null) {
            owner = GetComponentInParent<CharacterBase>();
        }

        Collider2D hitCollider = GetComponent<Collider2D>();

        if (hitCollider != null) {
            hitCollider.isTrigger = true;
        }
    }

    public void Setup(CharacterBase newOwner, BurgerLayerSO newLayer) {
        owner = newOwner;
        layer = newLayer;

        Collider2D hitCollider = GetComponent<Collider2D>();

        if (hitCollider != null) {
            hitCollider.isTrigger = true;
        }
    }
}
