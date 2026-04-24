using UnityEngine;

/// <summary>
/// EnemyBossStraight を変更せずに IBossTarget へ適合させるアダプタ。
/// EnemyBossStraight と同じ GameObject にアタッチする。
///
/// EnemyBossStraight.TakeDamage(int damage, Vector3 attackerPos) は
/// 攻撃者座標が必要なため、Mini 側から攻撃者座標を渡せる
/// TakeDamageWithBonusFrom(int, Vector3) も公開する。
/// </summary>
[RequireComponent(typeof(EnemyBossStraight))]
public class BossStraightAdapter : MonoBehaviour, IBossTarget
{
    EnemyBossStraight boss;
    int pendingBonus;

    public int PendingBonus => pendingBonus;

    void Awake()
    {
        boss = GetComponent<EnemyBossStraight>();
    }

    // ──────────────────────────────────────────
    // IBossTarget
    // ──────────────────────────────────────────

    public void AddDamageBonus(int bonus)
    {
        pendingBonus += bonus;
    }

    /// <summary>
    /// 攻撃者座標が不明な場合のフォールバック。
    /// attackerPos は this.transform.position（自身 = ボス位置）で代替する。
    /// </summary>
    public void TakeDamageWithBonus(int baseDamage)
    {
        TakeDamageWithBonusFrom(baseDamage, transform.position);
    }

    /// <summary>
    /// Mini の衝突処理から攻撃者座標を渡して呼ぶ推奨オーバーロード。
    /// EnemyBossStraight のフェーズ判定に必要な attackerPos を正しく渡せる。
    /// </summary>
    public void TakeDamageWithBonusFrom(int baseDamage, Vector3 attackerPos)
    {
        int total    = baseDamage + pendingBonus;
        pendingBonus = 0;
        boss.TakeDamage(total, attackerPos);
    }

    public void ClearBonus() => pendingBonus = 0;
}
