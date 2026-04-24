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
    [SerializeField, Tooltip("BossAlphaAdapter または BossStraightAdapter をアタッチした GameObject を指定")]
    MonoBehaviour bossTargetMono;
    IBossTarget bossTarget;

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>ランタイムでボス戦フラグを切り替えたい場合に使う。</summary>
    public void SetBossBattle(bool value) => isBossBattle = value;

    public void SetBossTarget(IBossTarget target) => bossTarget = target;

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void Awake()
    {
        // Inspector アサインした MonoBehaviour を IBossTarget にキャスト
        if (bossTargetMono != null)
            bossTarget = bossTargetMono as IBossTarget;

        if (bossTargetMono != null && bossTarget == null)
            Debug.LogWarning(
                "[ComboRewardApplier] bossTargetMono が IBossTarget を実装していません。" +
                "BossAlphaAdapter か BossStraightAdapter を設定してください。");
    }

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
        if (bossTarget == null) return;
        bossTarget.AddDamageBonus(step.damageBonus);
    }
}
