using UnityEngine;

/// <summary>
/// 出現から initialMoveDuration 秒かけて initialMoveDistance unit 移動 →
// stopDuration 秒停止 → dashSpeed で直進
/// enemy_ramNeedle で使用
/// </summary>
public class MoveAccele : MonoBehaviour, IEnemyComponent, IEnemyInitializable
{
	[Header("移動設定")]
	[SerializeField] private float entryAngle = 0f;
	[SerializeField] private float initialMoveDistance = 2f;  // 最初に移動する距離 [unit]
	[SerializeField] private float initialMoveDuration = 1f;  // 初期移動にかける時間 [秒]
	[SerializeField] private float stopDuration = 1f;         // 停止時間 [秒]
	[SerializeField] private float dashSpeed = 10f;           // ダッシュ速度 [unit/秒]

	private enum AcceleState { InitialMove, Stopping, Dashing }
	private AcceleState state = AcceleState.InitialMove;

	private EnemyBaseBeta core;
	private Vector3 moveDirection;
	private Vector3 initialStartPos;
	private float stateTimer = 0f;

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		this.core = core;   // core への参照を保存
		initialStartPos = transform.position;                                           // 初期位置を保存
		InitializeMovement(entryAngle);
	}

	public void Initialize(float direction)
	{
		// EnemySpawner から渡された方向で初期化
		InitializeMovement(direction);
	}

	private void InitializeMovement(float angle)
	{
		float rad = angle * Mathf.Deg2Rad;                                         // entryAngle から移動方向を計算（Y軸回転のみ）
		moveDirection = new Vector3(Mathf.Sin(rad), 0f, Mathf.Cos(rad)).normalized;     // 正規化して単位ベクトルにする
	}

	private void Update()
	{
		if (core == null || core.IsDead) return;

		stateTimer += Time.deltaTime;   // 状態ごとの処理

        switch (state)
		{
			// 初期移動
			case AcceleState.InitialMove:
				// initialMoveDuration 秒かけて initialMoveDistance unit を線形移動
				float t = Mathf.Clamp01(stateTimer / initialMoveDuration);                          // t は 0 から 1 までの割合
                transform.position = initialStartPos + moveDirection * (initialMoveDistance * t);   // 移動が完了したら次の状態へ

                if (stateTimer >= initialMoveDuration)
				{
					state = AcceleState.Stopping;
					stateTimer = 0f;
				}
				break;

            // 停止
            case AcceleState.Stopping:
				// 停止（何もしない）
				if (stateTimer >= stopDuration)
				{
					state = AcceleState.Dashing;
					stateTimer = 0f;
				}
				break;

            // ダッシュ
            case AcceleState.Dashing:
				transform.position += moveDirection * dashSpeed * Time.deltaTime;   // 直線移動
                break;
		}
	}
}