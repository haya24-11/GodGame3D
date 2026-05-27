using UnityEngine;

/// <summary>
/// 消滅時間の半分まで targetPosition へ moveSpeed で移動
/// その後、残り半分の時間で出現座標へ moveSpeed で移動
/// Y座標は出現時の値に固定される
/// enemy_zago で使用
/// </summary>
public class MovePopReturn : MonoBehaviour, IEnemyComponent, IEnemyInitializable
{
	[Header("移動設定")]
	[SerializeField] private Vector3 targetPosition;        // 到達目標座標
	[SerializeField] private float moveSpeed = 5f;          // 速度 [unit/秒]

	private enum PopState { MovingToTarget, ReturningToSpawn, Done }
	private PopState state = PopState.MovingToTarget;

	private EnemyBaseBeta core;
	private Vector3 returnPosition;
	private float despawnTime = 0f;
	private float elapsedTime = 0f;
	private float stateChangeTime = 0f;

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		this.core = core;                           // コアを記憶
		returnPosition = transform.position;	        // 出現座標を帰還先として記憶
	}

	public void Initialize(float direction, float despawnTime)
	{
		// EnemySpawner から方向情報と消滅時間を受け取る
		this.despawnTime = despawnTime;
		this.stateChangeTime = despawnTime * 0.5f; // 消滅時間の半分を状態変更時刻とする
		this.elapsedTime = 0f;

		// targetPosition のY座標を出現位置に固定
		targetPosition = new Vector3(targetPosition.x, returnPosition.y, targetPosition.z);
	}

	private void Update()
	{
		if (core == null || core.IsDead) return;

		elapsedTime += Time.deltaTime;

		switch (state)
		{
			// 消滅時間の前半：targetPosition へ移動
			case PopState.MovingToTarget:
				MoveToTarget();
				if (elapsedTime >= stateChangeTime)
				{
					state = PopState.ReturningToSpawn;
				}
				break;

			// 消滅時間の後半：出現座標へ帰還
			case PopState.ReturningToSpawn:
				MoveToReturn();
				break;

			// 完了
			case PopState.Done:
				ReturnToPool();
				break;
		}
	}

	// targetPosition へ moveSpeed で移動（Y座標は固定）
	private void MoveToTarget()
	{
		Vector3 targetPos = new Vector3(targetPosition.x, returnPosition.y, targetPosition.z);

		transform.position = Vector3.MoveTowards(
			transform.position,
			targetPos,
            moveSpeed * Time.deltaTime
        );
	}

	// 出現座標へ moveSpeed で移動（Y座標は固定）
	private void MoveToReturn()
	{
		Vector3 returnPos = new Vector3(returnPosition.x, returnPosition.y, returnPosition.z);

		transform.position = Vector3.MoveTowards(
			transform.position,
			returnPos,
            moveSpeed * Time.deltaTime
        );

        // 出現座標に到達したら完了
        if (Vector3.Distance(transform.position, returnPos) < 0.01f)
		{
			transform.position = returnPos;
			state = PopState.Done;
		}
	}

	private void ReturnToPool()
	{
		// オブジェクトプールが実装されたら差し替え
		// ObjectPool.Instance.Return(gameObject);
		Destroy(gameObject);
	}
}