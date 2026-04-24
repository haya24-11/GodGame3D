using UnityEngine;

/// <summary>
/// EnemyBossAlpha を変更せずに IBossTarget へ適合させるアダプタ。
/// EnemyBossAlpha と同じ GameObject にアタッチする。
///
/// EnemyBossAlpha.OnTriggerEnter は手を加えないため、
/// Mini 側の衝突処理を MiniAttackOnBoss に差し替えてこちらを呼ぶ。
/// </summary>
[RequireComponent(typeof(EnemyBossAlpha))]
public class BossAlphaAdapter : MonoBehaviour, IBossTarget
{
    EnemyBossAlpha boss;
    int pendingBonus;

    public int PendingBonus => pendingBonus;

    void Awake()
    {
        boss = GetComponent<EnemyBossAlpha>();
    }

    // ──────────────────────────────────────────
    // IBossTarget
    // ──────────────────────────────────────────

    public void AddDamageBonus(int bonus)
    {
        pendingBonus += bonus;
    }

    /// <summary>
    /// ボーナスを合算して EnemyBossAlpha.TakeDamage に委譲し、ボーナスを消費する。
    /// Mini の OnTriggerEnter の代わりにここを呼ぶ。
    /// </summary>
    public void TakeDamageWithBonus(int baseDamage)
    {
        int total    = baseDamage + pendingBonus;
        pendingBonus = 0;
        boss.TakeDamage(total);
    }

    public void ClearBonus() => pendingBonus = 0;
}
