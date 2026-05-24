using UnityEngine;

// ============================================
// ファイル：IDamageable.cs
// 役割：ダメージを受けられる対象
// ============================================

public interface IDamageable
{
    void TakeDamage(
        int damage,
        Vector3 attackerPos
    );
}