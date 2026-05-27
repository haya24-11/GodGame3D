using UnityEngine;

/// <summary>
/// enemy_tridera の Unit 本体
/// call（プレイヤーの遠距離攻撃）によって破壊される
/// mini の衝突ではダメージを受けない
/// </summary>
public class DoubleUnit : MonoBehaviour
{
	private UnitAttachment owner;

	public void Initialize(UnitAttachment owner)
	{
		this.owner = owner; // 破壊されたときに owner に通知するための参照
    }

	/// <summary>call（攻撃）を受けたとき 攻撃判定側から呼ぶ</summary>
	public void OnHitByCall()
	{
		owner?.OnUnitDestroyed(this);   // owner に破壊されたことを通知
        Destroy(gameObject);
	}

	// mini との衝突では Unit はダメージを受けない（仕様通り）
	private void OnTriggerEnter(Collider other) { }
}