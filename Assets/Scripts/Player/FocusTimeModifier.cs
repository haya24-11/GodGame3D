using UnityEngine;
using System;

/// <summary>
/// 制限時間の消費速度を管理するタイマー。
/// FocusSystem が集中中は消費速度が倍率分だけ上昇する。
/// 
/// 使い方:
///   float elapsed = focusTimeModifier.GetScaledDeltaTime();
///   remainingTime -= elapsed;
/// 
/// または OnTimeUp イベントを購読して残り時間ゼロを検知する。
/// </summary>
public class FocusTimeModifier : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] FocusSystem focusSystem;

    [Header("タイマー設定")]
    [SerializeField] float totalTime = 60f;

    // ──────────────────────────────────────────
    // Public properties
    // ──────────────────────────────────────────

    public float RemainingTime { get; private set; }
    public bool  IsRunning     { get; private set; }

    /// <summary>残り時間がゼロになったとき発火。</summary>
    public event Action OnTimeUp;

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void Awake()
    {
        RemainingTime = totalTime;
    }

    void Update()
    {
        if (!IsRunning) return;
        if (RemainingTime <= 0f) return;

        RemainingTime -= GetScaledDeltaTime();

        if (RemainingTime <= 0f)
        {
            RemainingTime = 0f;
            IsRunning     = false;
            OnTimeUp?.Invoke();
        }
    }

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    public void StartTimer()  => IsRunning = true;
    public void StopTimer()   => IsRunning = false;
    public void ResetTimer()  => RemainingTime = totalTime;

    /// <summary>
    /// コンボ報酬などから残り時間を加算する。
    /// totalTime を上限としてクランプする。
    /// </summary>
    public void AddTime(float amount)
    {
        RemainingTime = Mathf.Min(RemainingTime + amount, totalTime);
    }

    /// <summary>
    /// 集中中は倍率を乗じた deltaTime を返す。
    /// 外部タイマーと統合したい場合はこれを使う。
    /// </summary>
    public float GetScaledDeltaTime()
    {
        float multiplier = (focusSystem != null && focusSystem.IsFocusing)
                           ? focusSystem.TimeConsumptionMultiplier
                           : 1f;
        return Time.deltaTime * multiplier;
    }
}
