// ============================================
// ファイル：BossBase.cs
// 役割：全ボス共通処理をする
// 内容：HP / ダメージ / 死亡 / 点滅 / タイマー加算
// ============================================

using UnityEngine;
using System.Collections;

public abstract class BossBase : MonoBehaviour,IDamageable
{
    // ============================================
    // ステータス
    // ============================================

    [Header("ステータス")]
    [SerializeField]
    protected int maxHp = 10;

    protected int currentHp;

    // ============================================
    // 共通参照
    // ============================================

    protected Renderer rend;
    protected Color originalColor;

    protected Ttimer timerSystem;

    // ============================================
    // 状態
    // ============================================

    protected bool isDead = false;

    // ============================================
    // プロパティ
    // ============================================

    public int CurrentHp => currentHp;

    public event System.Action OnBossDead;
    // ============================================
    // 初期化
    // ============================================

    protected virtual void Start()
    {
        currentHp = maxHp;

        // ============================
        // Renderer取得
        // Visual側取得対応
        // ============================

        rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            originalColor = rend.material.color;
        }

        // ============================
        // タイマー取得
        // ============================

        timerSystem = FindObjectOfType<Ttimer>();

        if (timerSystem == null)
        {
            Debug.LogError("[BossBase] Ttimerが見つからない");
        }

        // ============================
        // HPバー接続
        // ============================

        var ui = FindObjectOfType<BossHPBarUI>();

        if (ui != null)
        {
            ui.Initialize(() => currentHp, maxHp);
        }
    }

    // ============================================
    // ダメージ
    // ============================================

    public virtual void TakeDamage(
        int damage,
        Vector3 attackerPos
    )
    {
        if (isDead) return;

        int beforeHp = currentHp;

        currentHp -= damage;

        Debug.Log(
            $"[{gameObject.name}] ダメージ:{damage} / HP:{beforeHp} → {currentHp}"
        );

        // ============================
        // 被弾演出
        // ============================

        StartCoroutine(DamageFlash());

        //EffectManager.Instance.PlayWeekpoint(transform.position);

        // ============================
        // ボス固有被弾処理
        // ============================

        OnDamaged(damage, attackerPos);

        // ============================
        // 死亡
        // ============================

        if (currentHp <= 0)
        {
            currentHp = 0;

            Die();
        }
    }

    // ============================================
    // ボス固有被弾処理
    // 各ボス側でoverride
    // ============================================

    protected virtual void OnDamaged(
        int damage,
        Vector3 attackerPos
    )
    {
    }

    // ============================================
    // 時間加算
    // ============================================

    protected void AddTime(int value)
    {
        if (timerSystem != null)
        {
            timerSystem.AddTime(value);

            Debug.Log(
                $"[{gameObject.name}] タイム+{value}"
            );
        }
    }

    // ============================================
    // 点滅
    // ============================================

    protected virtual IEnumerator DamageFlash()
    {
        if (rend == null)
        {
            yield break;
        }

        rend.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        rend.material.color = originalColor;
    }

    // ============================================
    // 撃破開始
    // 意図：全ボス共通の死亡入口
    // ============================================

    protected void Die()
    {
        if (isDead)
        {
            Debug.Log($"[{gameObject.name}] Dieは既に実行済み");
            return;
        }

        isDead = true;

        int listenerCount =
            OnBossDead != null
                ? OnBossDead.GetInvocationList().Length
                : 0;

        Debug.Log(
            $"[{gameObject.name}] OnBossDead 発火 / 登録数:{listenerCount}"
        );

        OnBossDead?.Invoke();

        StartCoroutine(DeathSequence());

        EffectManager.Instance.PlayEnemyDeath(transform.position);
    }

    // ============================================
    // 死亡処理
    // ============================================

    protected virtual IEnumerator DeathSequence()
    {
        Debug.Log($"[{gameObject.name}] 撃破！");

        // ============================
        // 見た目OFF
        // ============================

        if (rend != null)
        {
            rend.enabled = false;
        }

        // ============================
        // Collider OFF
        // ============================

        var col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        yield return new WaitForSeconds(1f);

        Debug.Log($"[{gameObject.name}] 死亡処理完了");
    }
}