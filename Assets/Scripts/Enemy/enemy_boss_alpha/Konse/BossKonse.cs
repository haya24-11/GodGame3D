// ============================================
// ファイル：BossKonse.cs
// 役割：無敵切替型ボス
// ============================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossKonse : BossBase
{
    // ============================================
    // 状態
    // ============================================

    private enum State
    {
        Protected,
        Exposed,
        Recover
    }

    private State state;

    // ============================================
    // Minion
    // ============================================

    [Header("Minion")]

    [SerializeField]
    private GameObject minionPrefab;

    [SerializeField]
    private int spawnCount = 3;

    [SerializeField]
    private float minionSpeed = 5f;

    private List<KonseMinion> activeMinions = new();

    private int aliveMinionCount = 0;

    // ============================================
    // 本体
    // ============================================

    [Header("本体")]

    [SerializeField]
    private float orbitRadius = 2f;

    [SerializeField]
    private float orbitSpeed = 60f;

    private float orbitAngle = 0f;

    // ============================================
    // 露出
    // ============================================

    private float exposedTimer = 0f;

    private int exposedDamage = 0;

    // ============================================
    // 強化段階
    // ============================================

    private int breakCount = 0;

    // ============================================
    // 被弾可能
    // ============================================

    private bool canTakeDamage = false;

    // ============================================
    // 初期化
    // ============================================

    protected override void Start()
    {
        base.Start();

        state = State.Protected;

        SpawnMinions();
    }

    // ============================================
    // 更新
    // ============================================

    void Update()
    {
        if (isDead) return;

        OrbitMove();

        switch (state)
        {
            case State.Protected:
                UpdateProtected();
                break;

            case State.Exposed:
                UpdateExposed();
                break;

            case State.Recover:
                break;
        }
    }

    // ============================================
    // 円移動
    // ============================================

    void OrbitMove()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;

        float rad = orbitAngle * Mathf.Deg2Rad;

        Vector3 center = Vector3.zero;

        Vector3 pos = new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            1f,
            Mathf.Sin(rad) * orbitRadius
        );

        transform.position = center + pos;
    }

    // ============================================
    // Minion生成
    // ============================================

    void SpawnMinions()
    {
        activeMinions.Clear();

        aliveMinionCount = spawnCount;

        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = Vector3.zero;

            // ====================================
            // 上下左右ランダム
            // ====================================

            int side = Random.Range(0, 4);

            switch (side)
            {
                case 0:
                    pos = new Vector3(
                        Random.Range(-w, w),
                        1f,
                        h
                    );
                    break;

                case 1:
                    pos = new Vector3(
                        Random.Range(-w, w),
                        1f,
                        -h
                    );
                    break;

                case 2:
                    pos = new Vector3(
                        w,
                        1f,
                        Random.Range(-h, h)
                    );
                    break;

                case 3:
                    pos = new Vector3(
                        -w,
                        1f,
                        Random.Range(-h, h)
                    );
                    break;
            }

            GameObject obj =
                ObjectPool.Instance.Get(
                    minionPrefab,
                    pos,
                    Quaternion.identity
                );

            KonseMinion minion =
                obj.GetComponent<KonseMinion>();

            minion.Init(this, minionPrefab, minionSpeed);

            activeMinions.Add(minion);
        }

        Debug.Log($"[Konse] Minion生成:{spawnCount}");
    }

    // ============================================
    // Minion死亡通知
    // ============================================

    public void NotifyMinionDead()
    {
        aliveMinionCount--;

        if (aliveMinionCount <= 0)
        {
            EnterExposed();
        }
    }

    // ============================================
    // 露出開始
    // ============================================

    void EnterExposed()
    {
        state = State.Exposed;

        canTakeDamage = true;

        exposedTimer = 10f;

        exposedDamage = 0;

        Debug.Log("[Konse] 本体露出");
    }

    // ============================================
    // 無敵状態
    // ============================================

    void UpdateProtected()
    {
    }

    // ============================================
    // 露出状態
    // ============================================

    void UpdateExposed()
    {
        exposedTimer -= Time.deltaTime;

        if (exposedTimer <= 0f)
        {
            StartCoroutine(RecoverRoutine());
        }
    }

    // ============================================
    // 回復フェーズ
    // ============================================

    IEnumerator RecoverRoutine()
    {
        state = State.Recover;

        canTakeDamage = false;

        yield return new WaitForSeconds(1f);

        SpawnMinions();

        state = State.Protected;
    }

    // ============================================
    // 被弾
    // ============================================

    protected override void OnDamaged(
        int damage,
        Vector3 attackerPos
    )
    {
        if (!canTakeDamage)
        {
            return;
        }

        exposedDamage += damage;

        Debug.Log(
            $"[Konse] 露出ダメージ:{exposedDamage}"
        );

        // ========================================
        // 4ダメージで吹っ飛び
        // ========================================

        if (exposedDamage >= 4)
        {
            breakCount++;

            AddTime(10);

            // 強化
            if (breakCount == 1)
            {
                minionSpeed = 7f;
            }
            else if (breakCount >= 2)
            {
                spawnCount++;
            }

            StartCoroutine(RecoverRoutine());
        }
    }
}