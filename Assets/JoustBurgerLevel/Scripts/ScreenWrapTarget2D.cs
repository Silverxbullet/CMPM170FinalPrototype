using UnityEngine;

namespace PrototypeFinalProject
{
    // Add to the SAME object as the future actor's Rigidbody2D.
    [DefaultExecutionOrder(10000)]
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody2D))]
    public sealed class ScreenWrapTarget2D : MonoBehaviour
    {
        [Tooltip("Assign ScreenWrapping from the arena. A single active arena in this scene is found automatically.")]
        [SerializeField] private HorizontalWrapArena arena;
        private Rigidbody2D body;
        private float nextSearchTime;

        public HorizontalWrapArena Arena { get => arena; set => arena = value; }

        private void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();
            nextSearchTime = 0f;
        }

        private void LateUpdate() { WrapNow(); }

        // Also callable by a movement system after manually simulating physics.
        public bool WrapNow()
        {
            if (!isActiveAndEnabled) return false;
            if (body == null) body = GetComponent<Rigidbody2D>();
            if (arena == null)
            {
                if (Application.isPlaying && Time.unscaledTime < nextSearchTime) return false;
                nextSearchTime = Time.unscaledTime + 1f;
                arena = FindArenaInThisScene();
            }
            return arena != null && arena.TryWrapBody(body);
        }

        private HorizontalWrapArena FindArenaInThisScene()
        {
            HorizontalWrapArena match = null;
            foreach (var candidate in Object.FindObjectsByType<HorizontalWrapArena>())
            {
                if (!candidate.isActiveAndEnabled || candidate.gameObject.scene != gameObject.scene) continue;
                if (match != null)
                {
                    Debug.LogWarning("More than one wrapping arena is active in this scene. Assign Arena on ScreenWrapTarget2D.", this);
                    return null;
                }
                match = candidate;
            }
            return match;
        }
    }
}
