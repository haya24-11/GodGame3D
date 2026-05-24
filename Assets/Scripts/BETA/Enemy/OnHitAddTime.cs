using UnityEngine;

/// <summary>
/// 被弾時に制限時間を加算するコンポーネント
/// enemy_pomoon で使用（被弾のたびに +10）
/// </summary>
public class OnHitAddTime : MonoBehaviour, IEnemyComponent
{
	[Header("加算設定")]
	[SerializeField] private int addSeconds = 10; // 被弾時に加算する秒数

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		core.OnDamaged += _ => core.AddTime(addSeconds);    // 被弾時に制限時間を加算
    }
}