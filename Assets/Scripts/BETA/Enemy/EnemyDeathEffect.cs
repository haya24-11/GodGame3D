using UnityEngine;

/// <summary>
/// 通常敵の撃破時に effect_enemy_death を再生するコンポーネント。
/// EnemyBaseBeta と同じ GameObject にアタッチする。
/// </summary>
public class EnemyDeathEffect : MonoBehaviour, IEnemyComponent
{
    public void OnEnemyInit(EnemyBaseBeta core)
    {
        core.OnDeath += PlayDeathEffect;
    }

    void PlayDeathEffect()
    {
        if (EffectManager.Instance == null) return;

        EffectManager.Instance.PlayEnemyDeath(transform.position);
    }
}
