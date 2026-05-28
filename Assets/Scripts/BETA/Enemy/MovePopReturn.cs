using UnityEngine;

/// <summary>
/// 出現後、指定された角度にまっすぐ進む → 消滅時間の半分経過後、出現座標へ帰還 → プール返却
/// Y座標は出現時の値に固定される
/// enemy_zago で使用
/// </summary>
public class MovePopReturn : MonoBehaviour, IEnemyComponent, IEnemyInitializable
{
	[Header("移動設定")]
	[SerializeField] private float moveSpeed = 5f;          // 速度 [unit/秒]

	private enum PopState { MovingToTarget, ReturningToSpawn, Done }
	private PopState state = PopState.MovingToTarget;

	private EnemyBaseBeta core;
	private Vector3 returnPosition;                         // 出現座標（帰還先）
	private float initialYPosition;                         // Y座標（固定用）
	private float direction = 0f;                           // 度数法の角度
	private float despawnTime = 0f;
	private float elapsedTime = 0f;
	private float stateChangeTime = 0f;

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		this.core = core;                                   // コアを記憶
		returnPosition = transform.position;                // 出現座標を帰還先として記憶
		initialYPosition = transform.position.y;            // Y座標を記憶（固定用）
	}

	public void Initialize(float direction,float despawnTime)
	{
		// EnemySpawner から方向情報と消滅時間を受け取る
		this.direction = direction;
        this.despawnTime = despawnTime;
        this.stateChangeTime = despawnTime * 0.5f; // 消滅時間の半分を状態変更時刻とする
        this.elapsedTime = 0f;
    }

	/// <summary>
	/// 消滅時間をセット (EnemySpawner から呼び出される)
	/// </summary>
	public void SetDespawnTime(float despawnTime)
	{
		this.despawnTime = despawnTime;
		this.stateChangeTime = despawnTime * 0.5f;  // 消滅時間の半分を状態変更時刻とする
		this.elapsedTime = 0f;
	}

	private void Update()
	{
		if (core == null || core.IsDead) return;
		if (despawnTime <= 0f) return;

		elapsedTime += Time.deltaTime;

		switch (state)
		{
			// 消滅時間の前半：指定角度にまっすぐ進む
			case PopState.MovingToTarget:
				MoveForward();
				if (elapsedTime >= stateChangeTime)
				{
					state = PopState.ReturningToSpawn;
					elapsedTime = 0f;
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

	/// <summary>
	/// 指定された角度にまっすぐ進む（Y座標は固定）
	/// </summary>
	private void MoveForward()
	{
		// 度数法から方向ベクトルに変換
		// 0度 = 真右 (+X方向), 90度 = 真上 (+Z方向)
		float radians = direction * Mathf.Deg2Rad;
		Vector3 moveDirection = new Vector3(
			Mathf.Sin(radians),
			0f,                  // Y方向は移動しない（固定）
			Mathf.Cos(radians)
		).normalized;

		// 移動
		transform.position += moveDirection * moveSpeed * Time.deltaTime;
	}

	/// <summary>
	/// 出現座標へ帰還（Y座標は固定）
	/// </summary>
	private void MoveToReturn()
	{
		Vector3 returnPos = new Vector3(returnPosition.x, initialYPosition, returnPosition.z);

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