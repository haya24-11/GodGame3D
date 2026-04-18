using UnityEngine;
using System;

/// <summary>
/// 1回の Call におけるヒット数を管理し、段階に応じた報酬イベントを発火する。
/// 報酬の実体付与は ComboRewardApplier が担うため、このクラスは付与処理を持たない。
/// 
/// ライフサイクル:
///   GameManager.StartCall() → BeginCall()
///   MiniMoveDispatcher が Mini ヒット時 → RegisterHit()
///   GameManager.OnAllMinisArrived() → EndCall()
/// </summary>
public class ComboCounter : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] ComboRewardConfig rewardConfig;

    // ──────────────────────────────────────────
    // Public properties
    // ──────────────────────────────────────────

    public int  HitCount      { get; private set; }
    public bool CallIsActive  { get; private set; }

    // ──────────────────────────────────────────
    // Events
    // ──────────────────────────────────────────

    /// <summary>
    /// ヒット時に発火。引数は「このヒットが何回目か（1始まり）」。
    /// </summary>
    public event Action<int> OnHit;

    /// <summary>
    /// 報酬付与が必要なヒットが発生したとき発火。
    /// 引数は RewardConfig.RewardStep（timeBonus / damageBonus を含む）。
    /// </summary>
    public event Action<ComboRewardConfig.RewardStep> OnRewardTriggered;

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>Call 開始時に GameManager から呼ぶ。カウンタをリセットする。</summary>
    public void BeginCall()
    {
        HitCount     = 0;
        CallIsActive = true;
    }

    /// <summary>
    /// Mini が target に到達（ヒット）したとき MiniMoveDispatcher から呼ぶ。
    /// 報酬条件を満たせば OnRewardTriggered を発火する。
    /// </summary>
    public void RegisterHit()
    {
        if (!CallIsActive) return;

        HitCount++;
        OnHit?.Invoke(HitCount);

        if (rewardConfig == null) return;

        var step = rewardConfig.Evaluate(HitCount);

        // 1回目は報酬ナシ（timeBonus=0 かつ damageBonus=0）のため発火しない
        bool hasReward = step.timeBonus > 0f || step.damageBonus > 0;
        if (hasReward)
            OnRewardTriggered?.Invoke(step);
    }

    /// <summary>Call 終了時（全 Mini 到着後）に GameManager から呼ぶ。</summary>
    public void EndCall()
    {
        CallIsActive = false;
    }
}
