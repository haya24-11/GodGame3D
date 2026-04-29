using UnityEngine;
using System;

/// <summary>
/// Ttimer を変更せずに集中システムの時間消費倍率を適用するアダプタ。
///
/// 仕組み:
///   Ttimer は「timer += Time.deltaTime → interval 超えたら cnttime--」で動く。
///   集中中は Ttimer.interval を (baseInterval / multiplier) に短縮することで
///   実質的な消費速度を倍率分だけ上げる。
///   集中解除時は baseInterval に戻す。
///
/// totalTime は Ttimer.cnttime の初期値（Start前の値）を自動取得する。
/// コンボ報酬の AddTime は Ttimer.AddTime(int) に委譲する。
/// </summary>
public class FocusTimeModifier : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] FocusSystem focusSystem;
    [SerializeField] Ttimer ttimer;

    // ──────────────────────────────────────────
    // Public properties（既存コードとのAPI互換）
    // ──────────────────────────────────────────

    /// <summary>残り時間（Ttimer.cnttime をそのまま返す）。</summary>
    public float RemainingTime => ttimer != null ? ttimer.cnttime : 0f;

    /// <summary>
    /// totalTime は Ttimer の初期 cnttime を起動時に記録した値。
    /// </summary>
    public float TotalTime { get; private set; }

    /// <summary>残り時間がゼロになったとき発火（Ttimer の0到達を監視）。</summary>
    public event Action OnTimeUp;

    // ──────────────────────────────────────────
    // Internal
    // ──────────────────────────────────────────

    float baseInterval;   // Ttimer の元の interval
    bool prevWasZero;    // OnTimeUp の二重発火防止

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void Awake()
    {
        if (ttimer == null) return;

        // Ttimer変更前の初期値を totalTime として記録
        TotalTime = ttimer.cnttime;
        baseInterval = ttimer.interval;
    }

    void OnEnable()
    {
        if (focusSystem == null) return;
        focusSystem.OnFocusStarted += ApplyFocusInterval;
        focusSystem.OnFocusEnded += ResetInterval;

        if (focusSystem.IsFocusing) ApplyFocusInterval();
    }

    void OnDisable()
    {
        if (focusSystem == null) return;
        focusSystem.OnFocusStarted -= ApplyFocusInterval;
        focusSystem.OnFocusEnded -= ResetInterval;
        ResetInterval();
    }

    void Update()
    {
        if (ttimer == null) return;

        // cnttime がゼロになった瞬間だけ OnTimeUp を発火
        bool isZero = ttimer.cnttime <= 0;
        if (isZero && !prevWasZero)
            OnTimeUp?.Invoke();
        prevWasZero = isZero;
    }

    // ──────────────────────────────────────────
    // Public API（ComboRewardApplier などから呼ばれる）
    // ──────────────────────────────────────────

    /// <summary>
    /// 残り時間を加算する。Ttimer.AddTime(int) に委譲。
    /// TotalTime（初期値）を上限としてクランプする。
    /// </summary>
    public void AddTime(float amount)
    {
        if (ttimer == null) return;

        int intAmount = Mathf.RoundToInt(amount);
        int clamped = Mathf.Min(ttimer.cnttime + intAmount, (int)TotalTime)
                        - ttimer.cnttime;

        if (clamped > 0)
            ttimer.AddTime(clamped);
    }

    /// <summary>
    /// 集中中かどうかで倍率を乗じた deltaTime を返す。
    /// 外部から手動で消費量を計算したい場合に使う。
    /// </summary>
    public float GetScaledDeltaTime()
    {
        float multiplier = (focusSystem != null && focusSystem.IsFocusing)
                           ? focusSystem.TimeConsumptionMultiplier
                           : 1f;
        return Time.deltaTime * multiplier;
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    void ApplyFocusInterval()
    {
        if (ttimer == null) return;
        float multiplier = focusSystem != null ? focusSystem.TimeConsumptionMultiplier : 1f;
        // interval を短くすることで単位時間あたりのカウントダウン回数を増やす
        ttimer.interval = baseInterval / multiplier;
    }

    void ResetInterval()
    {
        if (ttimer != null)
            ttimer.interval = baseInterval;
    }
}
