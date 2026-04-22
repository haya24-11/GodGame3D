using UnityEngine;
using UnityEngine.InputSystem;

public class InfiniteFieldScroller : MonoBehaviour
{
    [Header("スクロール方向・速度")]
    public Vector2 scrollDirection = new Vector2(0f, 1f);
    public float scrollSpeed = 0.5f;

    [Header("テクスチャ拡大率")]
    public Vector2 tiling = new Vector2(1f, 1f);

    [Header("区切りで方向転換")]
    public bool enableDirectionChange = true;
    public Vector2[] directionSequence = new Vector2[]
    {
        new Vector2( 0f,  1f),
        new Vector2( 1f,  0f),
        new Vector2( 0f, -1f),
        new Vector2(-1f,  0f),
    };

    [Header("キーボード設定 (Key名で指定)")]
    [Tooltip("例: UpArrow / W / Space など UnityEngine.InputSystem.Key の名前を入力")]
    public string keyUp = "UpArrow";
    public string keyDown = "DownArrow";
    public string keyLeft = "LeftArrow";
    public string keyRight = "RightArrow";

    private Key _keyUp, _keyDown, _keyLeft, _keyRight;

    private Material mat;
    private float prevOffsetX = 0f;
    private float prevOffsetY = 0f;

    private Vector2? pendingDirection = null;
    private bool keyControlled = false;
    private int _sequenceIndex = 0;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.mainTextureScale = tiling;

        _keyUp = ParseKey(keyUp, Key.UpArrow);
        _keyDown = ParseKey(keyDown, Key.DownArrow);
        _keyLeft = ParseKey(keyLeft, Key.LeftArrow);
        _keyRight = ParseKey(keyRight, Key.RightArrow);

        if (directionSequence.Length > 0)
            scrollDirection = directionSequence[0];
    }

    void Update()
    {
        HandleKeyInput();

        Vector2 dir = scrollDirection.normalized;

        // 負方向のときはabs値でオフセット計算し、常に 0→1 方向に増加させる
        float offsetX = Mathf.Repeat(Time.time * Mathf.Abs(dir.x) * scrollSpeed, 1f);
        float offsetY = Mathf.Repeat(Time.time * Mathf.Abs(dir.y) * scrollSpeed, 1f);

        // テクスチャの見た目は符号で反転
        float texOffsetX = dir.x >= 0f ? offsetX : 1f - offsetX;
        float texOffsetY = dir.y >= 0f ? offsetY : 1f - offsetY;

        // ループ検出は常に 0→1 増加する offsetX/Y で行うので正負どちらでも動作する
        bool loopedX = Mathf.Abs(dir.x) > 0f && DetectLoop(prevOffsetX, offsetX);
        bool loopedY = Mathf.Abs(dir.y) > 0f && DetectLoop(prevOffsetY, offsetY);
        bool onBoundary = loopedX || loopedY;

        if (onBoundary)
        {
            if (pendingDirection.HasValue)
            {
                ApplyDirectionInternal(pendingDirection.Value);
                pendingDirection = null;
                keyControlled = true;
            }
            else if (enableDirectionChange && !keyControlled && directionSequence.Length > 1)
            {
                _sequenceIndex = (_sequenceIndex + 1) % directionSequence.Length;
                scrollDirection = directionSequence[_sequenceIndex];
            }
        }

        mat.mainTextureOffset = new Vector2(texOffsetX, texOffsetY);
        mat.mainTextureScale = tiling;

        prevOffsetX = offsetX;
        prevOffsetY = offsetY;
    }

    private void HandleKeyInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb[_keyUp].wasPressedThisFrame) { pendingDirection = Vector2.up; keyControlled = false; }
        if (kb[_keyDown].wasPressedThisFrame) { pendingDirection = Vector2.down; keyControlled = false; }
        if (kb[_keyLeft].wasPressedThisFrame) { pendingDirection = Vector2.left; keyControlled = false; }
        if (kb[_keyRight].wasPressedThisFrame) { pendingDirection = Vector2.right; keyControlled = false; }
    }

    private void ApplyDirectionInternal(Vector2 newDir)
    {
        scrollDirection = newDir;
        for (int i = 0; i < directionSequence.Length; i++)
        {
            if (Vector2.Distance(directionSequence[i].normalized, newDir.normalized) < 0.01f)
            {
                _sequenceIndex = i;
                break;
            }
        }
    }

    public void ApplyDirection(Vector2 newDir)
    {
        pendingDirection = newDir;
        keyControlled = false;
    }

    public void SetScrollSpeed(float speed) => scrollSpeed = speed;
    public void SetDirectionUp() => ApplyDirection(Vector2.up);
    public void SetDirectionDown() => ApplyDirection(Vector2.down);
    public void SetDirectionLeft() => ApplyDirection(Vector2.left);
    public void SetDirectionRight() => ApplyDirection(Vector2.right);

    private Key ParseKey(string name, Key fallback)
    {
        if (System.Enum.TryParse<Key>(name, true, out Key result))
            return result;
        Debug.LogWarning($"[InfiniteFieldScroller] キー名 '{name}' が無効です。{fallback} を使用します。");
        return fallback;
    }

    private bool DetectLoop(float prev, float current)
    {
        return prev > 0.9f && current < 0.1f;
    }
}