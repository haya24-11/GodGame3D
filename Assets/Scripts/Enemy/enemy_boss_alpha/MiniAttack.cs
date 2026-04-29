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

        var cubehit = other.GetComponentInParent<enemy_hp>();
        if (cubehit != null) {
            Debug.Log("[Mini] cubehitにヒット");
            cubehit.TakeDamage(attack);
            return;
        }

       /* var enemyhit = other.GetComponentInParent<EnemyBase>();
        if (enemyhit != null)
        {
            Debug.Log("[Mini] accelehitにヒット");
            enemyhit.TakeDamage(attack);
            return;
        }
       */
        var accelehit = other.GetComponentInParent<EnemyAccele>();
        if (accelehit != null)
        {
            Debug.Log("[Mini] accelehitにヒット");
            accelehit.TakeDamage(attack);
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