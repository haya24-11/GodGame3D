using UnityEngine;

/// <summary>
/// 被弾時に画面内の全敵（ボス除く）を Destroy するコンポーネント
/// enemy_pomoon で使用
/// </summary>
public class BombEffect : MonoBehaviour, IEnemyComponent
{
	[Header("爆発設定")]
	[SerializeField] private string[] excludeTags = { "Boss", "enemy_bomb" };

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		core.OnDamaged += _ => DestroyAllEnemiesOnScreen(); // 被弾時に全敵を破壊
	}

	private void DestroyAllEnemiesOnScreen()
	{
		Camera cam = Camera.main;   // 画面内判定用（null の場合は全て破壊）

		// シーン内の全ての EnemyBaseBeta をチェック
		foreach (var enemy in Object.FindObjectsByType<EnemyBaseBeta>(FindObjectsSortMode.None))
		{
			if (enemy.gameObject == this.gameObject) continue;							// 自分自身は除外
			if (IsExcluded(enemy.gameObject)) continue;                                 // タグで除外
			if (cam != null && !IsOnScreen(enemy.transform.position, cam)) continue;    // 画面外の敵は除外

			Destroy(enemy.gameObject);
		}
	}

    // タグが excludeTags に含まれているか
    private bool IsExcluded(GameObject obj)
	{
		foreach (string tag in excludeTags)
			if (obj.CompareTag(tag)) return true;
		return false;
	}

    // worldPos が cam の画面内にあるか
    private bool IsOnScreen(Vector3 worldPos, Camera cam)
	{
		Vector3 vp = cam.WorldToViewportPoint(worldPos);
		return vp.z > 0f
			&& vp.x >= 0f && vp.x <= 1f
			&& vp.y >= 0f && vp.y <= 1f;
	}
}