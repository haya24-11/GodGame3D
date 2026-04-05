using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    void Update()
    {
        MoveInput = Gamepad.current != null ? Gamepad.current.leftStick.ReadValue() : Vector2.zero;
    }
}
