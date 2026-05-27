using System;
using UnityEngine;

/// <summary>
/// 全敵の核となるコンポーネント（EnemyCore）
/// HP管理・被弾・死亡のみを責務とし
/// 移動・効果などは別コンポーネントがイベントを購読して実装する
/// </summary>
public class EnemyBaseBeta : MonoBehaviour
{
	// ――――――――――――――――――――――――――――――――――――
	// インスペクター
	// ――――――――――――――――――――――――――――――――――――

	[Header("基本ステータス")]
	[SerializeField] private int maxHp = 2;

	// ――――――――――――――――――――――――――――――――――――
	// 公開イベント（他コンポーネントが購読する）
	// ――――――――――――――――――――――――――――――――――――

	/// <summary>被弾時 引数 = 受けたダメージ量</summary>
	public event Action<int> OnDamaged;

	/// <summary>死亡時（Destroy 直前）</summary>
	public event Action OnDeath;

	// ――――――――――――――――――――――――――――――――――――
	// 内部状態
	// ――――――――――――――――――――――――――――――――――――

	private int currentHp;
	private bool isDead = false;

	// ――――――――――――――――――――――――――――――――――――
	// Unity ライフサイクル
	// ――――――――――――――――――――――――――――――――――――

	private void Awake()
	{
		currentHp = maxHp;
	}

	private void Start()
	{
		// 同一 GameObject にアタッチされた全コンポーネントに初期化を通知
		foreach (var comp in GetComponents<IEnemyComponent>())	// IEnemyComponent を実装したコンポーネントを全て取得
		{
			comp.OnEnemyInit(this);		// 各コンポーネントの初期化メソッドを呼び出す
		}
	}

	// ――――――――――――――――――――――――――――――――――――
	// 公開 API
	// ――――――――――――――――――――――――――――――――――――

	public bool IsDead => isDead;
	public int CurrentHp => currentHp;
	public int MaxHp => maxHp;

	/// <summary>ダメージを受ける HP が 0 以下になると Die() を呼ぶ</summary>
	public void TakeDamage(int damage)
	{
		if (isDead || damage <= 0) return;

		currentHp -= damage;
		OnDamaged?.Invoke(damage);	// ダメージ量を引数にしてイベントを発火

		if (currentHp <= 0)
			Die();
	}

	/// <summary>GameManager 経由で制限時間を加算するヘルパー 各コンポーネントから呼ぶ</summary>
	public void AddTime(int seconds)
	{
		Ttimer ttimer = FindObjectOfType<Ttimer>(includeInactive: false);
		if (ttimer != null)
		{
			ttimer.AddTime(seconds);
		}
		else
		{
			Debug.LogWarning($"[EnemyBaseBeta] Ttimer not found");
		}

		//if (GameManager.Instance != null)
		//    GameManager.Instance.AddTime(seconds);
	}

	// ――――――――――――――――――――――――――――――――――――
	// 内部処理
	// ――――――――――――――――――――――――――――――――――――

	private void Die()
	{
		if (isDead) return;
		isDead = true;
		
		OnDeath?.Invoke();		// 死亡イベントを発火
		Destroy(gameObject);
	}

	// ――――――――――――――――――――――――――――――――――――
	// mini との衝突（物理）
	// ――――――――――――――――――――――――――――――――――――

	private void OnTriggerEnter(Collider other)
	{
        // MiniAttackOnBoss コンポーネントを持つ Mini か確認
        MiniAttackOnBoss miniAttacker = other.GetComponent<MiniAttackOnBoss>();
        if (miniAttacker == null) return;

        // Mini がアクティブ状態であることを確認（念のため）
        if (!miniAttacker.IsActive) return;

        // Mini の攻撃力を取得してダメージを与える
        TakeDamage(miniAttacker.Attack);

        //MiniController mini = other.GetComponent<MiniController>();
        //if (mini != null)
        //    TakeDamage(mini.AttackPower);
    }
}