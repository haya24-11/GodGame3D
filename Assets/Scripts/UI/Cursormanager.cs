using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class StageSelectCursor : MonoBehaviour
{
    [SerializeField] private RectTransform[] stageButtons;
    [SerializeField] private RectTransform cursorRect;

    private int currentIndex = 0;

    void Start()
    {
        MoveCursor();
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // ç∂âEà⁄ìÆ
        if (gamepad.dpad.right.wasPressedThisFrame ||
            gamepad.leftStick.right.wasPressedThisFrame)
            MoveIndex(+1);

        if (gamepad.dpad.left.wasPressedThisFrame ||
            gamepad.leftStick.left.wasPressedThisFrame)
            MoveIndex(-1);

        // åàíË
        if (gamepad.buttonSouth.wasPressedThisFrame)
            stageButtons[currentIndex].GetComponent<Button>().onClick.Invoke();
    }

    private void MoveIndex(int dir)
    {
        int next = Mathf.Clamp(currentIndex + dir, 0, stageButtons.Length - 1);
        if (next == currentIndex) return;
        currentIndex = next;
        MoveCursor();
    }

    private void MoveCursor()
    {
        Vector3 targetPos = stageButtons[currentIndex].position;
        targetPos.y -= 80f;
        cursorRect.position = targetPos;
    }
}
























