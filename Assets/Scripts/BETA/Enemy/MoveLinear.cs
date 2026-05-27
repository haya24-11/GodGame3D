using UnityEngine;

/// <summary>
/// 直進移動コンポーネント
/// 入射角から移動方向を計算し、一定速度で直進し続ける
/// enemy_genghis / enemy_tridera / enemy_pomoon で使用
/// </summary>
public class MoveLinear : MonoBehaviour, IEnemyComponent, IEnemyInitializable
{
	[Header("直進設定")]
	[SerializeField] private float entryAngle = 0f;   // 画面への入射角（度）
	[SerializeField] private float moveSpeed = 5f;    // 速度 [unit/秒]

	private Vector3 moveDirection;
	private EnemyBaseBeta core;

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		this.core = core;   // コアへの参照を保持（移動停止のため）
		InitializeMovement(entryAngle);
	}

	public void Initialize(float direction, float despawnTime)
	{
		// EnemySpawner から渡された方向と消滅時間で初期化
		// このコンポーネントでは despawnTime は使用しないが、インターフェース実装のため定義
		InitializeMovement(direction);
	}

	private void InitializeMovement(float angle)
	{
		float rad = angle * Mathf.Deg2Rad;										// 入射角から移動方向を計算（X-Z平面での移動を想定）
		moveDirection = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)).normalized;	// 入射角0度は正Z方向、90度は正X方向、-90度は負X方向になるように計算
	}

	private void Update()
	{
		if (core == null || core.IsDead) return;
		transform.position += moveDirection * moveSpeed * Time.deltaTime;   // 毎フレーム、moveDirection へ moveSpeed で移動
	}
}