using UnityEngine;

/// <summary>
/// Mini が Boss に衝突したときの攻撃処理。
/// IBossTarget 経由でダメージを与えることで、コンボボーナスが自動適用される。
///
/// アタッチ先: Mini Prefab
///
/// EnemyBossAlpha.OnTriggerEnter の既存処理と競合しないよう、
/// EnemyBossAlpha 側の OnTriggerEnter は残したまま、
/// こちらが先に IBossTarget.TakeDamageWithBonus を呼ぶ。
///
/// ※ EnemyBossAlpha.OnTriggerEnter も同じ衝突で TakeDamage を呼ぶため
///   ダメージが二重になる場合は EnemyBossAlpha の Tag 判定を
///   "MiniLegacy" 等に変えるか、OnTriggerEnter を無効化すること。
/// </summary>
public class MiniAttackOnBoss : MonoBehaviour
{
    [SerializeField, Tooltip("Mini の攻撃力（ベース値）")]
    int attack = 2;

    public int Attack => attack;

    void OnTriggerEnter(Collider other)
    {
        // IBossTarget を実装したアダプタを持つボスか確認
        var bossTarget = other.GetComponent<IBossTarget>();
        if (bossTarget == null) return;

        // BossStraightAdapter なら攻撃者座標も渡せる推奨ルートを使う
        var straightAdapter = other.GetComponent<BossStraightAdapter>();
        if (straightAdapter != null)
            straightAdapter.TakeDamageWithBonusFrom(attack, transform.position);
        else
            bossTarget.TakeDamageWithBonus(attack);

        // Mini 自身は Destroy（MiniMoveDispatcher の OnArrived より先に来た場合の保険）
        // ※ 通常は ArrivalDetector → MiniMoveDispatcher が Destroy するため二重 Destroy に注意
        // Destroy(gameObject);
    }
}
