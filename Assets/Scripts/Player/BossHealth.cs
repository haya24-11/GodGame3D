using UnityEngine;
using System;

/// <summary>
/// ボスの HP 管理クラス。
/// コンボシステムからの damageBonus を蓄積し、
/// 次の TakeDamage 呼び出し時に上乗せして消費する。
/// </summary>
public class BossHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 1000;

    public int  CurrentHP      { get; private set; }
    public int  PendingBonus   { get; private set; }

    public event Action       OnDead;
    public event Action<int>  OnDamageTaken; // 引数: 実際に与えたダメージ量

    void Awake()
    {
        CurrentHP = maxHP;
    }

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>
    /// コンボ報酬から呼ばれる。蓄積ボーナスに加算する（消費はしない）。
    /// </summary>
    public void AddDamageBonus(int bonus)
    {
        PendingBonus += bonus;
    }

    /// <summary>
    /// 通常の攻撃ダメージを与える。
    /// PendingBonus が蓄積されていれば合算して消費する。
    /// </summary>
    public void TakeDamage(int baseDamage)
    {
        int total = baseDamage + PendingBonus;
        PendingBonus = 0;

        CurrentHP = Mathf.Max(0, CurrentHP - total);
        OnDamageTaken?.Invoke(total);

        if (CurrentHP == 0)
            OnDead?.Invoke();
    }

    /// <summary>蓄積ボーナスを明示的にリセットしたい場合に使う。</summary>
    public void ClearBonus() => PendingBonus = 0;
}
