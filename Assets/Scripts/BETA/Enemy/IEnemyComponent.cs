using UnityEngine;

/// <summary>
/// EnemyBaseBeta にアタッチされる全コンポーネントの基底インターフェース
/// EnemyBaseBeta が初期化・イベント登録のタイミングを統一するために使用する
/// </summary>
public interface IEnemyComponent
{
	/// <summary>
	/// EnemyBaseBeta の Start 後に呼ばれる初期化
	/// GetComponent などは Awake より後であることが保証される
	/// </summary>
	void OnEnemyInit(EnemyBaseBeta core);
}