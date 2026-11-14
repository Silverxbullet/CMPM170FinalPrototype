using UnityEngine;

namespace PrototypeFinalProject
{
    [DisallowMultipleComponent]
    public sealed class HorizontalWrapArena : MonoBehaviour
    {
        [SerializeField] private Transform leftBoundary;
        [SerializeField] private Transform rightBoundary;
        [SerializeField, Min(0.1f)] private float gizmoHeight = 20f;

        public void SetBoundaries(Transform left, Transform right)
        {
            leftBoundary = left;
            rightBoundary = right;
        }

        public bool TryGetWrappedPosition(Vector2 position, out Vector2 wrapped)
        {
            wrapped = position;
            if (leftBoundary == null || rightBoundary == null) return false;
            float left = leftBoundary.position.x;
            float right = rightBoundary.position.x;
            if (!IsFinite(left) || !IsFinite(right) || right <= left ||
                !IsFinite(position.x) || !IsFinite(position.y)) return false;
            if (position.x >= left && position.x <= right) return false;

            // Keep overshoot, even if movement crosses several arena widths.
            // Double precision avoids rounding a very small left-edge crossing to right.
            double width = (double)right - left;
            double offset = ((double)position.x - left) % width;
            if (offset < 0d) offset += width;
            wrapped.x = (float)(left + offset);
            return true;
        }

        public bool TryWrapBody(Rigidbody2D body)
        {
            if (!isActiveAndEnabled || body == null || !body.simulated ||
                body.bodyType == RigidbodyType2D.Static ||
                !TryGetWrappedPosition(body.position, out Vector2 wrapped)) return false;

            Vector2 velocity = body.linearVelocity;
            float spin = body.angularVelocity;
            // Teleport rather than MovePosition: do not sweep across the arena.
            body.position = wrapped;
            body.linearVelocity = velocity;
            body.angularVelocity = spin;
            return true;
        }

        private static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }

        private void OnDrawGizmos()
        {
            if (leftBoundary == null || rightBoundary == null) return;
            Gizmos.color = Color.cyan;
            Vector3 up = transform.up * (gizmoHeight * 0.5f);
            Gizmos.DrawLine(leftBoundary.position - up, leftBoundary.position + up);
            Gizmos.DrawLine(rightBoundary.position - up, rightBoundary.position + up);
        }
    }
}
