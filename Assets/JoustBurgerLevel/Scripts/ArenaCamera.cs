using UnityEngine;

namespace PrototypeFinalProject
{
    // Frames the entire 32 x 20 arena, including in narrow Game windows.
    [ExecuteAlways, RequireComponent(typeof(Camera))]
    public sealed class ArenaCamera : MonoBehaviour
    {
        [SerializeField, Min(0.1f)] private float arenaWidth = 32f;
        [SerializeField, Min(0.1f)] private float arenaHeight = 20f;
        private Camera arenaCamera;

        private void OnEnable() { arenaCamera = GetComponent<Camera>(); Fit(); }
        private void LateUpdate() { Fit(); }
        private void Fit()
        {
            if (arenaCamera == null || arenaCamera.aspect <= 0f) return;
            arenaCamera.orthographic = true;
            float size = Mathf.Max(arenaHeight * 0.5f, arenaWidth / (2f * arenaCamera.aspect));
            if (!Mathf.Approximately(arenaCamera.orthographicSize, size))
                arenaCamera.orthographicSize = size;
        }
    }
}
