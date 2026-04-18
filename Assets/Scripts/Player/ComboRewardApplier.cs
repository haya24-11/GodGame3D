using UnityEngine;

/// <summary>
/// ComboCounter の OnRewardTriggered を受け取り、報酬を実際に付与する。
/// ボス戦かどうかを IsBossBattle フラグで切り替える。
/// 
/// 通常戦 : FocusTimeModifier の RemainingTime に timeBonus を加算
/// ボス戦  : BossHealth.AddDamageBonus() で damageBonus を蓄積し、
///           次の攻撃ダメージに上乗せする
/// </summary>
public class ComboRewardApplier : UnityEngine.MonoBehaviour
{
    [Header("モード")]
    [SerializeField, Tooltip("true = ボス戦モード（時間ボーナスなし・ダメージボーナスあり）")]
    bool isBossBattle = false;

    [Header("参照")]
    [SerializeField] ComboCounter      comboCounter;
    [SerializeField] FocusTimeModifier focusTimeModifier; // 通常戦のみ使用
    [SerializeField] BossHealth        bossHealth;        // ボス戦のみ使用

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>ランタイムでボス戦フラグを切り替えたい場合に使う。</summary>
    public void SetBossBattle(bool value) => isBossBattle = value;

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void OnEnable()
    {
        if (comboCounter != null)
            comboCounter.OnRewardTriggered += ApplyReward;
    }

    void OnDisable()
    {
        if (comboCounter != null)
            comboCounter.OnRewardTriggered -= ApplyReward;
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    void ApplyReward(ComboRewardConfig.RewardStep step)
    {
        if (isBossBattle)
            ApplyBossReward(step);
        else
            ApplyNormalReward(step);
    }

    void ApplyNormalReward(ComboRewardConfig.RewardStep step)
    {
        if (focusTimeModifier == null) return;
        focusTimeModifier.AddTime(step.timeBonus);
    }

    void ApplyBossReward(ComboRewardConfig.RewardStep step)
    {
        if (bossHealth == null) return;
        bossHealth.AddDamageBonus(step.damageBonus);
    }
}
