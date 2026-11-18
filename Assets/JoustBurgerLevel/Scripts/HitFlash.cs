using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    public Color flash_color = Color.white;
    public float flash_duration = 0.12f;

    private SpriteRenderer sprite_renderer;
    private Color normal_color;

    private void Awake()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
        normal_color = sprite_renderer.color;
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        sprite_renderer.color = flash_color;
        yield return new WaitForSeconds(flash_duration);
        sprite_renderer.color = normal_color;
    }
}
