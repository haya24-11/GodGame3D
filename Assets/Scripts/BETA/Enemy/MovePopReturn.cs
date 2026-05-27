using UnityEngine;

/// <summary>
/// 指定座標へ速度 moveSpeed で到達 → 指定秒数待機 → 出現座標へ帰還 → プール返却
/// enemy_zago で使用
/// </summary>
public class MovePopReturn : MonoBehaviour, IEnemyComponent
{
	[Header("移動設定")]
	[SerializeField] private Vector3 targetPosition;        // 到達目標座標
	[SerializeField] private float moveSpeed = 5f;          // 速度 [unit/秒]
	[SerializeField] private float arrivalWaitSeconds = 1f; // 到達後の待機時間 [秒]

	private enum PopState { MovingToTarget, Waiting, ReturningToSpawn, Done }
	private PopState state = PopState.MovingToTarget;

	private EnemyBaseBeta core;
	private Vector3 returnPosition;
	private float waitTimer = 0f;

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		this.core = core;                       // コアを記憶
		returnPosition = transform.position;	// 出現座標を帰還先として記憶
	}

	private void Update()
	{
		if (core == null || core.IsDead) return;

		switch (state)
		{
			// 到達目標へ移動
			case PopState.MovingToTarget:
				MoveToward(targetPosition, PopState.Waiting);
				break;

			// 到達後の待機
			case PopState.Waiting:
				waitTimer += Time.deltaTime;
				if (waitTimer >= arrivalWaitSeconds)
					state = PopState.ReturningToSpawn;
				break;

			// 出現座標へ帰還
			case PopState.ReturningToSpawn:
				MoveToward(returnPosition, PopState.Done);
				break;

			// プール返却
			case PopState.Done:
				ReturnToPool();
				break;
		}
	}

	// destination へ移動し、到達したら nextState へ遷移
	private void MoveToward(Vector3 destination, PopState nextState)
	{
		transform.position = Vector3.MoveTowards(   // 現在位置から
			transform.position,						// 目的地へ向かって
			destination,							// 速度 moveSpeed で移動
            moveSpeed * Time.deltaTime				// [unit/秒] * [秒] = [unit]
        );

        // 目的地に十分近づいたら位置を目的地に合わせて次の状態へ
        if (Vector3.Distance(transform.position, destination) < 0.01f)
		{
			transform.position = destination;
			waitTimer = 0f;
			state = nextState;
		}
	}

	private void ReturnToPool()
	{
		// オブジェクトプールが実装されたら差し替え
		// ObjectPool.Instance.Return(gameObject);
		Destroy(gameObject);
	}
}