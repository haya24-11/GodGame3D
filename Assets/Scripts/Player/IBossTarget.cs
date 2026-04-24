/// <summary>
/// コンボシステム・集中システムがボスを参照するための統一インターフェース。
/// EnemyBossAlpha / EnemyBossStraight を変更せず、
/// アダプタ経由でこのインターフェースに適合させる。
/// </summary>
public interface IBossTarget
{
    /// <summary>
    /// コンボ報酬のダメージボーナスを蓄積する。
    /// 次の TakeDamageWithBonus 呼び出し時に消費される。
    /// </summary>
    void AddDamageBonus(int bonus);

    /// <summary>
    /// 蓄積ボーナスを上乗せしてダメージを与える。
    /// Mini の OnTriggerEnter からこちらを呼ぶことで
    /// コンボボーナスが自動適用される。
    /// </summary>
    void TakeDamageWithBonus(int baseDamage);

    /// <summary>現在蓄積されているボーナス値。</summary>
    int PendingBonus { get; }
}
