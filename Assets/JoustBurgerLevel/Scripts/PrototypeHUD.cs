using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PrototypeHUD : MonoBehaviour
{
    public PlayerController player;

    private void Update() {
        if (player == null) {
            player = FindAnyObjectByType<PlayerController>();
        }

        if (player != null && !player.IsAlive && Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void OnGUI() {
        if (player == null) {
            return;
        }

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 24;

        GUI.Label(
            new Rect(20, 20, 700, 40),
            "Burger layers: " + player.LayerCount,
            style);

        GUI.Label(
            new Rect(20, 55, 900, 40),
            "A/D or arrows: move    Space: jump    Left click: attack",
            style);

        if (!player.IsAlive) {
            style.fontSize = 36;
            GUI.Label(
                new Rect(20, 100, 900, 60),
                "GAME OVER - press R to restart",
                style);
        }
    }
}