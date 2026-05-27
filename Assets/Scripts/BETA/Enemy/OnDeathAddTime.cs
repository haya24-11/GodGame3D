using UnityEngine;

/// <summary>
/// 死亡時に制限時間を加算するコンポーネント
/// enemy_genghis / enemy_zago / enemy_tridera / enemy_ramNeedle で使用
/// </summary>
public class OnDeathAddTime : MonoBehaviour, IEnemyComponent
{
	[Header("加算設定")]
	[SerializeField] private int addSeconds = 1; // 死亡時に加算する秒数

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		core.OnDeath += () => core.AddTime(addSeconds); // 死亡時に制限時間を加算
    }
}