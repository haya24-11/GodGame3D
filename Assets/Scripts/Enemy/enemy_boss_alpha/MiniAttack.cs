// どのファイルのどこを変更：MiniAttack.cs
// 意図：ボスにダメージ＋方向を渡す

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MiniAttack : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private int attack = 2;

    public int Attack => attack;

    // ★追加：衝突時にダメージを与える
    private void OnTriggerEnter(Collider other)
    {
        // boss_straight用
        var bossStraight = other.GetComponentInParent<EnemyBossStraight>();
        if (bossStraight != null)
        {
            Debug.Log("[Mini] BossStraightにヒット");
            bossStraight.TakeDamage(attack, transform.position);
            return;
        }

        // boss_alpha用（既存維持）
        var bossAlpha = other.GetComponent<EnemyBossAlpha>();
        if (bossAlpha != null)
        {
            bossAlpha.TakeDamage(attack);
            return;
        }
    }
}