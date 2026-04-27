using UnityEngine;
using UnityEngine.InputSystem;

public class InfiniteFieldScroller : MonoBehaviour
{
    [Header("スクロール方向・速度")]
    public Vector2 scrollDirection = new Vector2(0f, 1f);
    public float scrollSpeed = 0.5f;

    [Header("テクスチャ拡大率")]
    public Vector2 tiling = new Vector2(1f, 1f);

    [Header("テクスチャリスト")]
    public Texture2D[] textures;
    public bool autoChangeTextureOnLoop = false;

    [Header("正面待ち実行")]
    [Tooltip("ONにすると、方向・画像の切替を次の正面タイミングまで保留する")]
    public bool waitForFrontPosition = true;

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
    public string keyUp = "UpArrow";
    public string keyDown = "DownArrow";
    public string keyLeft = "LeftArrow";
    public string keyRight = "RightArrow";

    [Header("テクスチャ切替キー (省略可)")]
    public string keyNextTexture = "Tab";
    public string keyPrevTexture = "";

    private Key _keyUp, _keyDown, _keyLeft, _keyRight;
    private Key _keyNextTex = Key.None;
    private Key _keyPrevTex = Key.None;

    private Material mat;
    private float prevOffsetX = 0f;
    private float prevOffsetY = 0f;

    private Vector2? pendingDirection = null;
    private int? pendingTextureIndex = null;  // ★ 保留中のテクスチャ
    private bool keyControlled = false;
    private int _sequenceIndex = 0;
    private int _textureIndex = 0;
    private bool _boundaryHandledThisLoop = false;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.mainTextureScale = tiling;

        _keyUp = ParseKey(keyUp, Key.UpArrow);
        _keyDown = ParseKey(keyDown, Key.DownArrow);
        _keyLeft = ParseKey(keyLeft, Key.LeftArrow);
        _keyRight = ParseKey(keyRight, Key.RightArrow);

        if (!string.IsNullOrEmpty(keyNextTexture))
            _keyNextTex = ParseKey(keyNextTexture, Key.Tab);
        if (!string.IsNullOrEmpty(keyPrevTexture))
            _keyPrevTex = ParseKey(keyPrevTexture, Key.None);

        if (directionSequence.Length > 0)
            scrollDirection = directionSequence[0];

        ApplyTexture(_textureIndex);
    }

    void Update()
    {
        HandleKeyInput();

        Vector2 dir = scrollDirection.normalized;

        float offsetX = Mathf.Repeat(Time.time * Mathf.Abs(dir.x) * scrollSpeed, 1f);
        float offsetY = Mathf.Repeat(Time.time * Mathf.Abs(dir.y) * scrollSpeed, 1f);

        float texOffsetX = dir.x >= 0f ? offsetX : 1f - offsetX;
        float texOffsetY = dir.y >= 0f ? offsetY : 1f - offsetY;

        bool loopedX = Mathf.Abs(dir.x) > 0f && DetectLoop(prevOffsetX, offsetX);
        bool loopedY = Mathf.Abs(dir.y) > 0f && DetectLoop(prevOffsetY, offsetY);
        bool onBoundary = loopedX || loopedY;

        if (!onBoundary)
        {
            _boundaryHandledThisLoop = false;
        }
        else if (!_boundaryHandledThisLoop)
        {
            _boundaryHandledThisLoop = true;

            // ★ 保留中のテクスチャを正面タイミングで適用
            if (pendingTextureIndex.HasValue)
            {
                ApplyTexture(pendingTextureIndex.Value);
                _textureIndex = pendingTextureIndex.Value;
                pendingTextureIndex = null;
            }
            else if (autoChangeTextureOnLoop)
            {
                ApplyTexture((_textureIndex + 1) % textures.Length);
                _textureIndex = (_textureIndex + 1) % textures.Length;
            }

            // 保留中の方向を正面タイミングで適用
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

    // ── キー入力（いつでも受け付ける） ───────────────
    private void HandleKeyInput()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        // 方向キー：即座に pendingDirection に積む
        if (kb[_keyUp].wasPressedThisFrame) { pendingDirection = Vector2.up; keyControlled = false; }
        if (kb[_keyDown].wasPressedThisFrame) { pendingDirection = Vector2.down; keyControlled = false; }
        if (kb[_keyLeft].wasPressedThisFrame) { pendingDirection = Vector2.left; keyControlled = false; }
        if (kb[_keyRight].wasPressedThisFrame) { pendingDirection = Vector2.right; keyControlled = false; }

        // テクスチャ切替キー：即座に pendingTextureIndex に積む
        if (_keyNextTex != Key.None && kb[_keyNextTex].wasPressedThisFrame)
            QueueTexture(_textureIndex + 1);
        if (_keyPrevTex != Key.None && kb[_keyPrevTex].wasPressedThisFrame)
            QueueTexture(_textureIndex - 1);
    }

    // ── テクスチャ操作 ────────────────────────────────
    /// <summary>次の正面タイミングで切り替わるよう予約する</summary>
    private void QueueTexture(int index)
    {
        if (textures == null || textures.Length == 0) return;
        pendingTextureIndex = ((index % textures.Length) + textures.Length) % textures.Length;
    }

    /// <summary>外部から番号指定で予約（waitForFrontPosition が OFF なら即時）</summary>
    public void SetTexture(int index)
    {
        if (waitForFrontPosition) QueueTexture(index);
        else
        {
            int i = ((index % textures.Length) + textures.Length) % textures.Length;
            _textureIndex = i;
            ApplyTexture(i);
        }
    }

    public void SetNextTexture() => SetTexture(_textureIndex + 1);
    public void SetPrevTexture() => SetTexture(_textureIndex - 1);

    private void ApplyTexture(int index)
    {
        if (textures == null || textures.Length == 0) return;
        var tex = textures[index];
        if (tex == null) return;
        mat.mainTexture = tex;
        mat.mainTextureScale = tiling;
    }

    // ── 方向操作 ──────────────────────────────────────
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

    public void ApplyDirection(Vector2 newDir) { pendingDirection = newDir; keyControlled = false; }

    public void SetScrollSpeed(float speed) => scrollSpeed = speed;
    public void SetDirectionUp() => ApplyDirection(Vector2.up);
    public void SetDirectionDown() => ApplyDirection(Vector2.down);
    public void SetDirectionLeft() => ApplyDirection(Vector2.left);
    public void SetDirectionRight() => ApplyDirection(Vector2.right);

    // ── ユーティリティ ────────────────────────────────
    private Key ParseKey(string name, Key fallback)
    {
        if (string.IsNullOrEmpty(name)) return fallback;
        if (System.Enum.TryParse<Key>(name, true, out Key result)) return result;
        Debug.LogWarning($"[InfiniteFieldScroller] キー名 '{name}' が無効です。{fallback} を使用します。");
        return fallback;
    }

    private bool DetectLoop(float prev, float current) => prev > 0.9f && current < 0.1f;
}