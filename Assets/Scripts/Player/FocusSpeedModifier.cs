using UnityEngine;

/// <summary>
/// FocusSystem の状態に連動して Mover の移動速度を倍率変更する。
/// 
/// 使い方（2通り）：
///   A) Inspector で FocusSystem を直接アサイン（Cursor など静的オブジェクト用）
///   B) MiniSpawner から AddComponent → InjectFocusSystem()（動的スポーン用）
/// </summary>
[RequireComponent(typeof(Mover))]
public class FocusSpeedModifier : MonoBehaviour
{
    [Header("参照（静的アタッチ時のみ設定）")]
    [SerializeField] FocusSystem focusSystem;

    Mover mover;
    float baseMoveSpeed;
    bool  initialized;

    // ──────────────────────────────────────────
    // Public API（動的スポーン用）
    // ──────────────────────────────────────────

    /// <summary>
    /// MiniSpawner など外部から AddComponent 後に呼ぶ初期化メソッド。
    /// </summary>
    public void InjectFocusSystem(FocusSystem system)
    {
        focusSystem = system;
        Initialize();
    }

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void Awake()
    {
        mover         = GetComponent<Mover>();
        baseMoveSpeed = mover.MoveSpeed;
    }

    void OnEnable()
    {
        // Inspector アサイン時はここで初期化
        if (focusSystem != null && !initialized)
            Initialize();
    }

    void OnDisable()
    {
        if (focusSystem == null) return;
        focusSystem.OnFocusStarted -= ApplyFocusSpeed;
        focusSystem.OnFocusEnded   -= ResetSpeed;
        initialized = false;
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    void Initialize()
    {
        if (focusSystem == null || initialized) return;

        focusSystem.OnFocusStarted += ApplyFocusSpeed;
        focusSystem.OnFocusEnded   += ResetSpeed;
        initialized = true;

        // 既にフォーカス中なら即適用
        if (focusSystem.IsFocusing) ApplyFocusSpeed();
        else                        ResetSpeed();
    }

    void ApplyFocusSpeed()
        => mover.MoveSpeed = baseMoveSpeed * focusSystem.MoveSpeedMultiplier;

    void ResetSpeed()
        => mover.MoveSpeed = baseMoveSpeed;
}
