using UnityEngine;
using UnityEngine.UI;

public class StageSelectCursor : MonoBehaviour
{
    [SerializeField] private RectTransform[] stageButtons;
    [SerializeField] private RectTransform cursorRect;
    private int currentIndex = 0;
    private bool wasStickMoved = false;

    void Start()
    {
        MoveCursor();
    }

    void Update()
    {
        bool moveRight = false;
        bool moveLeft = false;
        bool decide = false;

        // ─── キーボード ───────────────────────────
        if (Input.GetKeyDown(KeyCode.RightArrow)) moveRight = true;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) moveLeft = true;
        if (Input.GetKeyDown(KeyCode.Return)) decide = true;

        // ─── コントローラー（XInput） ─────────────
        // 十字キー
        float dpadX = Input.GetAxisRaw("Horizontal");
        if (dpadX > 0.5f && !wasStickMoved) { moveRight = true; wasStickMoved = true; }
        else if (dpadX < -0.5f && !wasStickMoved) { moveLeft = true; wasStickMoved = true; }
        else if (Mathf.Abs(dpadX) < 0.3f) wasStickMoved = false;

        // Aボタンで決定
        if (Input.GetButtonDown("Submit")) decide = true;

        // ─── 処理 ─────────────────────────────────
        if (moveRight && currentIndex < stageButtons.Length - 1)
        {
            currentIndex++;
            MoveCursor();
        }
        if (moveLeft && currentIndex > 0)
        {
            currentIndex--;
            MoveCursor();
        }
        if (decide)
        {
            stageButtons[currentIndex].GetComponent<Button>().onClick.Invoke();
        }
    }

    private void MoveCursor()
    {
        Vector3 targetPos = stageButtons[currentIndex].position;
        targetPos.y -= 80f;
        cursorRect.position = targetPos;
    }
}