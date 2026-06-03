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

    //  Minionのサイズ（中心から端までの距離） 本体と重ならないようにするために必要
    [SerializeField]
    private float minionSize = 1f;

    private enum SpawnSide
    {   // Minionが出現する辺
        Top,
        Bottom,
        Right,
        Left
    }

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
        if (minionPrefab == null)
        {
            Debug.LogError("[Konse] minionPrefab 未設定");
            return;
        }

        if (ObjectPool.Instance == null)
        {
            Debug.LogError("[Konse] ObjectPool.Instance が存在しない");
            return;
        }

        activeMinions.Clear();
        aliveMinionCount = spawnCount;

        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        // =========================
        // 出現する辺を1回だけ決める
        // =========================

        SpawnSide side = (SpawnSide)Random.Range(0, 4);

        // =========================
        // ミニオン1体分の空白を空ける
        // 中心間距離 = 本体1体分 + 空白1体分
        // =========================

        float spacing = minionSize * 2f;

        float totalLength = spacing * (spawnCount - 1);
        float startOffset = -totalLength * 0.5f;

        float halfSize = minionSize * 0.5f;

        Vector3 moveDir = Vector3.zero;

        for (int i = 0; i < spawnCount; i++)
        {
            float offset = startOffset + spacing * i;

            Vector3 pos = Vector3.zero;

            switch (side)
            {
                case SpawnSide.Top:
                    // 上端に横1列配置 → 下へ進む
                    pos = new Vector3(
                        offset,
                        1f,
                        h - halfSize
                    );

                    moveDir = Vector3.back;
                    break;

                case SpawnSide.Bottom:
                    // 下端に横1列配置 → 上へ進む
                    pos = new Vector3(
                        offset,
                        1f,
                        -h + halfSize
                    );

                    moveDir = Vector3.forward;
                    break;

                case SpawnSide.Right:
                    // 右端に縦1列配置 → 左へ進む
                    pos = new Vector3(
                        w - halfSize,
                        1f,
                        offset
                    );

                    moveDir = Vector3.left;
                    break;

                case SpawnSide.Left:
                    // 左端に縦1列配置 → 右へ進む
                    pos = new Vector3(
                        -w + halfSize,
                        1f,
                        offset
                    );

                    moveDir = Vector3.right;
                    break;
            }

            GameObject obj = ObjectPool.Instance.Get(
                minionPrefab,
                pos,
                Quaternion.identity
            );

            KonseMinion minion = obj.GetComponent<KonseMinion>();

            if (minion == null)
            {
                Debug.LogError("[Konse] KonseMinion がPrefabに付いてない");
                return;
            }

            minion.Init(
                this,
                minionPrefab,
                minionSpeed,
                moveDir
            );

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
    public override void TakeDamage(
    int damage,
    Vector3 attackerPos
)
    {
        if (!canTakeDamage)
        {
            return;
        }

        base.TakeDamage(
            damage,
            attackerPos
        );
    }
}