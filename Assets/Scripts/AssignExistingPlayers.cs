using UnityEngine;
using UnityEngine.InputSystem;

public class AssignExistingPlayers : MonoBehaviour
{
    public PlayerInput player1;
    public PlayerInput player2;

    private void Start()
    {
        if (Gamepad.all.Count >= 1)
            player1.SwitchCurrentControlScheme("Gamepad", Gamepad.all[0]);

        if (Gamepad.all.Count >= 2)
            player2.SwitchCurrentControlScheme("Gamepad", Gamepad.all[1]);

        if (player2 == null) return;
    }
}