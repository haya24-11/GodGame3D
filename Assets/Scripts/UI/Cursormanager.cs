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
        // 右に移動
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            if (currentIndex < stageButtons.Length - 1)
            {
                currentIndex++;
                MoveCursor();
            }
        }

        // 左に移動
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            if (currentIndex > 0)
            {
                currentIndex--;
                MoveCursor();
            }
        }

        // Enterで決定
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            stageButtons[currentIndex].GetComponent<Button>().onClick.Invoke();
        }
    }

    private void MoveCursor()
    {
        Vector3 targetPos = stageButtons[currentIndex].position;
        targetPos.y -= 80f; // カーソルをボタンの下に表示
        cursorRect.position = targetPos;
    }
}