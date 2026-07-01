using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ResultButtonSelectorHorizontal : MonoBehaviour
{
    [System.Serializable]
    public class SelectableButton
    {
        public Button button;          // onClick.Invoke 対象
        public Image targetImage;      // テクスチャ差し替え対象（未指定なら button.image を使う）
        public Sprite normalSprite;    // 通常時のテクスチャ
        public Sprite selectedSprite;  // 選択中のテクスチャ
    }

    [SerializeField] private SelectableButton[] buttons; // 配列順 = 左から右

    private int currentIndex = 0;

    void Start()
    {
        ApplyHighlight();
    }

    void Update()
    {
        var gamepad = Gamepad.current;
        if (gamepad == null) return;

        // 左移動
        if (gamepad.dpad.left.wasPressedThisFrame ||
            gamepad.leftStick.left.wasPressedThisFrame)
            MoveIndex(-1);

        // 右移動
        if (gamepad.dpad.right.wasPressedThisFrame ||
            gamepad.leftStick.right.wasPressedThisFrame)
            MoveIndex(+1);

        // 決定（Aボタン）
        if (gamepad.buttonSouth.wasPressedThisFrame)
        {
            var current = buttons[currentIndex].button;
            if (current != null)
                current.onClick.Invoke();
        }
    }

    private void MoveIndex(int dir)
    {
        int next = Mathf.Clamp(currentIndex + dir, 0, buttons.Length - 1);
        if (next == currentIndex) return;
        currentIndex = next;
        ApplyHighlight();
    }

    private void ApplyHighlight()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var entry = buttons[i];
            Image img = entry.targetImage != null
                ? entry.targetImage
                : (entry.button != null ? entry.button.image : null);
            if (img == null) continue;

            img.sprite = (i == currentIndex) ? entry.selectedSprite : entry.normalSprite;
        }
    }
}
