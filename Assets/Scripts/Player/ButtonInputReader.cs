using UnityEngine;
using UnityEngine.InputSystem;

public class ButtonInputReader : MonoBehaviour
{
    public bool XButtonDown {  get; private set; }
    public bool YButtonDown { get; private set; }
    public bool RBDown      { get; private set; }
    public bool RTDown      { get; private set; }
    public bool LBDown      {  get; private set; }
    public bool LTDown      { get; private set; }

    void Update()
    {
        if (Gamepad.current == null) return;

        Gamepad gp = Gamepad.current;

        XButtonDown = gp.buttonWest.wasPressedThisFrame;
        YButtonDown = gp.buttonNorth.wasPressedThisFrame;
        RBDown      = gp.rightShoulder.wasPressedThisFrame;
        RTDown      = gp.rightTrigger.wasPressedThisFrame;
        LBDown      = gp.leftShoulder.wasPressedThisFrame;
        LTDown      = gp.leftTrigger.wasPressedThisFrame;
    }
}
