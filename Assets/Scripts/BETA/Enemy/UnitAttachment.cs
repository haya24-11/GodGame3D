using UnityEngine;

/// <summary>
/// enemy_tridera 専用コンポーネント
/// 本体の上下 or 左右に 1/2 スケールの Unit を 2 つ生成・管理する
/// 両 Unit が破壊されると EnemyBaseBeta.TakeDamage で本体を即死させる
/// </summary>
public class UnitAttachment : MonoBehaviour, IEnemyComponent
{
	// ―――――――――――――――――――――――――――――――
	// インスペクター
	// ―――――――――――――――――――――――――――――――

	[Header("Unit 設定")]
	
	[Tooltip("Unit の往復速度 [unit/秒]")]
	[SerializeField] private float unitMoveSpeed = 3f;

	[Tooltip("Unit が往復する最大距離（本体から） [unit]")]
	[SerializeField] private float unitTravelDistance = 1f;

	[Tooltip("折り返し時のフェード時間 [秒]（0 でフェードなし）")]
	[SerializeField] private float unitFadeDuration = 0.2f;

	[Tooltip("Unit プレハブ（null の場合は実行時に Capsule で生成）")]
	[SerializeField] private GameObject unitPrefab;

	// ―――――――――――――――――――――――――――――――
	// 内部状態
	// ―――――――――――――――――――――――――――――――

	private EnemyBaseBeta core;
	private DoubleUnit unit1;
	private DoubleUnit unit2;

    // 進行方向に対する左右ベクトル（OnEnemyInit で確定）
    private Vector3 rightDir;

    // Unit ごとの往復位相（0 = 本体に最近接, 1 = 最遠）
    private float phase1 = 1f;
	private float phase2 = 1f; // 逆位相でスタート

	private bool dir1 = true;  // true = 離れる方向
	private bool dir2 = false;

	// ―――――――――――――――――――――――――――――――
	// 初期化
	// ―――――――――――――――――――――――――――――――

	public void OnEnemyInit(EnemyBaseBeta core)
	{
		this.core = core;   // core への参照を保存

        rightDir = transform.right;

        // 初期位置に Unit を生成
        unit1 = SpawnUnit(rightDir * unitTravelDistance);
		unit2 = SpawnUnit(-rightDir * unitTravelDistance);

		// 死亡時に残存 Unit を掃除
		core.OnDeath += CleanupUnits;
	}

    // ――――――――――――――――――――――――――――――――――――
    // Unity ライフサイクル
    // ――――――――――――――――――――――――――――――――――――

    /// <summary>
    /// 本体が死亡・プール返却どちらの場合でも
    /// GameObject が非アクティブになると呼ばれる。
    /// Unit の残留を防ぐためここで掃除する。
    /// </summary>
    private void OnDisable()
    {
        CleanupUnits();

        // イベント購読も解除（プール再利用時の二重購読防止）
        if (core != null)
        {
            core.OnDeath -= CleanupUnits;
            core = null;
        }
    }

    // ―――――――――――――――――――――――――――――――
    // 毎フレーム処理
    // ―――――――――――――――――――――――――――――――

    private void Update()
	{
		if (core == null || core.IsDead) return;

        // Unit を往復移動させる
        if (unit1 != null) UpdateUnitPosition(unit1.transform, rightDir, ref phase1, ref dir1);
		if (unit2 != null) UpdateUnitPosition(unit2.transform, -rightDir, ref phase2, ref dir2);
	}

	/// <summary>
	/// phase を 0-1 で往復させ フェードで折り返し付近の速度を落とす
	/// </summary>
	private void UpdateUnitPosition(Transform unitTr, Vector3 baseDir, ref float phase, ref bool goingOut)
	{
		// 折り返し付近をフェードで減速
		float edge = Mathf.Min(phase, 1f - phase); // 0 に近いほど端
		// unitFadeDuration > 0 なら edge/unitFadeDuration で速度をスケール（最大1）、0 なら常に1
        float speedScale = unitFadeDuration > 0f
			? Mathf.Clamp01(edge / unitFadeDuration)
			: 1f;

        // phase を速度 unitMoveSpeed で更新（goingOut が true なら増加、false なら減少）
        float delta = (unitMoveSpeed / (unitTravelDistance * 2f)) * speedScale * Time.deltaTime;

        // goingOut が true なら phase を増加させ、1 以上になったら 1 に固定して goingOut を false にする
        if (goingOut)
		{
			phase += delta;
			if (phase >= 1f) { phase = 1f; goingOut = false; }
		}
		else
		{
			phase -= delta;
			if (phase <= 0f) { phase = 0f; goingOut = true; }
		}

        // phase を元に unit の位置を更新（baseDir 方向に phase * unitTravelDistance 離れる）
        unitTr.position = transform.position + baseDir * (phase * unitTravelDistance);
	}

	// ―――――――――――――――――――――――――――――――
	// Unit 破壊コールバック
	// ―――――――――――――――――――――――――――――――

	public void OnUnitDestroyed(DoubleUnit destroyed)
	{
		if (destroyed == unit1) unit1 = null;
		if (destroyed == unit2) unit2 = null;

		// 両方破壊 → 本体を即死
		if (unit1 == null && unit2 == null)
			core.TakeDamage(core.MaxHp);
	}

	// ―――――――――――――――――――――――――――――――
	// 生成・クリーンアップ
	// ―――――――――――――――――――――――――――――――

	private DoubleUnit SpawnUnit(Vector3 offset)
	{
        // unitPrefab が指定されていればそれをインスタンス化、なければ Capsule を生成
        GameObject obj = unitPrefab != null
			? Instantiate(unitPrefab, transform.position + offset, Quaternion.identity)
			: GameObject.CreatePrimitive(PrimitiveType.Capsule);

        // 位置とスケールを設定（スケールは本体の半分）
        obj.transform.position = transform.position + offset;
		obj.transform.localScale = transform.localScale * 0.5f;

		var unit = obj.AddComponent<DoubleUnit>();  // DoubleUnit コンポーネントを追加
        unit.Initialize(this);
		return unit;
	}

	private void CleanupUnits()
	{
		if (unit1 != null) Destroy(unit1.gameObject);
		if (unit2 != null) Destroy(unit2.gameObject);
	}
}