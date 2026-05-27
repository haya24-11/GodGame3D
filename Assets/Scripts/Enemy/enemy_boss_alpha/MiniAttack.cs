// ============================================
// ファイル：MiniAttack.cs
// 役割：Miniの攻撃処理
// 内容：IDamageableへダメージ送信
// ============================================

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MiniAttack : MonoBehaviour
{
    // ============================================
    // 攻撃力
    // ============================================

    [Header("攻撃力")]

    [SerializeField]
    private int attack = 2;

    public int Attack => attack;

    // ============================================
    // 衝突
    // ============================================

    private void OnTriggerEnter(Collider other)
    {
        // ========================================
        // MiniAttackOnBoss確認
        // ========================================

        MiniAttackOnBoss attackState =
            GetComponent<MiniAttackOnBoss>();

        if (attackState == null)
        {
            return;
        }

        if (!attackState.IsActive)
        {
            return;
        }

        // ========================================
        // IDamageable取得
        // ========================================

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        // ========================================
        // ダメージ
        // ========================================

        Debug.Log(
            $"[Mini] {other.name} にヒット"
        );

        damageable.TakeDamage(
            attack,
            transform.position
        );
    }
}