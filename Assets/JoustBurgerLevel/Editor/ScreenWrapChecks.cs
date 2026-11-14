using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeFinalProject.Editor
{
    public static class ScreenWrapChecks
    {
        [MenuItem("Tools/Prototype Final Project/Test Screen Wrapping")]
        private static void Run()
        {
            if (Application.isPlaying)
            {
                Debug.LogWarning("Stop Play Mode before running the wrapping checks.");
                return;
            }
            Scene previous = SceneManager.GetActiveScene();
            Scene scratch = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                var root = new GameObject("Temporary wrapping checks");
                var arena = root.AddComponent<HorizontalWrapArena>();
                Transform left = new GameObject("Left").transform;
                Transform right = new GameObject("Right").transform;
                left.SetParent(root.transform); right.SetParent(root.transform);
                left.position = new Vector3(-16f, 0f); right.position = new Vector3(16f, 0f);
                arena.SetBoundaries(left, right);

                float[] inside = { -16f, 0f, 16f };
                foreach (float x in inside)
                    Require(!arena.TryGetWrappedPosition(new Vector2(x, 3f), out _), "An inside point wrapped.");
                float[,] crossings = { {16.25f, -15.75f}, {-16.25f, 15.75f},
                    {49f, -15f}, {-49f, 15f}, {80.5f, -15.5f} };
                for (int i = 0; i < crossings.GetLength(0); i++)
                {
                    Require(arena.TryGetWrappedPosition(new Vector2(crossings[i, 0], 3f), out Vector2 p), "Crossing did not wrap.");
                    Near(p.x, crossings[i, 1], "Overshoot"); Near(p.y, 3f, "Height");
                }
                Require(!arena.TryGetWrappedPosition(new Vector2(float.NaN, 0f), out _), "NaN accepted.");
                Require(!arena.TryGetWrappedPosition(new Vector2(float.PositiveInfinity, 0f), out _), "Infinity accepted.");
                right.position = left.position;
                Require(!arena.TryGetWrappedPosition(new Vector2(20f, 0f), out _), "Zero-width arena accepted.");
                right.position = new Vector3(16f, 0f);

                var actor = new GameObject("Temporary body (no player)");
                var body = actor.AddComponent<Rigidbody2D>();
                body.gravityScale = 0f;
                var target = actor.AddComponent<ScreenWrapTarget2D>();
                // No assigned arena: verify automatic binding within this temporary scene.
                body.position = new Vector2(16.25f, 3f);
                body.rotation = 22f;
                body.linearVelocity = new Vector2(6f, -2f);
                body.angularVelocity = 15f;
                Require(target.WrapNow(), "Target failed to bind or wrap.");
                Near(body.position.x, -15.75f, "Body X"); Near(body.position.y, 3f, "Body height");
                Near(body.linearVelocity.x, 6f, "Horizontal momentum");
                Near(body.linearVelocity.y, -2f, "Vertical momentum");
                Near(body.angularVelocity, 15f, "Spin"); Near(body.rotation, 22f, "Rotation");
                Require(!target.WrapNow(), "Body wrapped twice without crossing.");
                arena.enabled = false; body.position = new Vector2(17f, 3f);
                Require(!target.WrapNow(), "Disabled arena wrapped a body.");
                arena.enabled = true; body.simulated = false;
                Require(!target.WrapNow(), "Unsimulated body wrapped.");
                body.simulated = true; body.bodyType = RigidbodyType2D.Static;
                Require(!target.WrapNow(), "Static body wrapped.");

                left.position = new Vector3(-6f, 0f); right.position = new Vector3(26f, 0f);
                Require(arena.TryGetWrappedPosition(new Vector2(26.25f, -4f), out Vector2 shifted), "Moved boundaries failed.");
                Near(shifted.x, -5.75f, "Moved boundary overshoot"); Near(shifted.y, -4f, "Moved boundary height");
                Debug.Log("Screen wrapping checks passed: both directions, overshoot, fast crossings, boundary points, invalid bounds, actor binding, height, momentum, rotation, disabled/static bodies and moved boundaries. Temporary objects removed; no player required.");
            }
            finally
            {
                if (previous.IsValid() && previous.isLoaded) SceneManager.SetActiveScene(previous);
                EditorSceneManager.CloseScene(scratch, true);
            }
        }

        private static void Near(float actual, float expected, string label)
        {
            Require(Mathf.Abs(actual - expected) < 0.0001f, label + ": expected " + expected + ", got " + actual);
        }

        private static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("Screen wrapping check failed: " + message);
        }
    }
}
