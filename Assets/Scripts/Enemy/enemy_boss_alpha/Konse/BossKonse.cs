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

    private enum SpawnLineType
    {   //  Minionが出現するラインのタイプ
        Center,
        CornerA,
        CornerB
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

        // =========================
        // 中央だけでなく、角寄りにも配置する
        // Center  : 中央
        // CornerA : 左上 / 下左 / 右上 / 左上側
        // CornerB : 右上 / 下右 / 右下 / 左下側
        // =========================

        SpawnLineType lineType = (SpawnLineType)Random.Range(0, 3);

        float lineCenter = 0f;  // ラインの中心位置（0が中央、負が左上/下、正が右上/下）

        switch (lineType)
        {   //  今回は中央に配置することが多め
            case SpawnLineType.Center:
                lineCenter = 0f;
                break;
            //  角寄りに配置することもある
            case SpawnLineType.CornerA:
                lineCenter = -Mathf.Min(w, h) * 0.5f;
                break;
            //  角寄りに配置することもある
            case SpawnLineType.CornerB:
                lineCenter = Mathf.Min(w, h) * 0.5f;
                break;
        }

        float halfSize = minionSize * 0.5f;

        Vector3 moveDir = Vector3.zero;

        for (int i = 0; i < spawnCount; i++)
        {
            float offset = startOffset + spacing * i;

            Vector3 pos = Vector3.zero;

            Vector3 waveDir = Vector3.zero;

            switch (side)
            {
                case SpawnSide.Top:
                    pos = new Vector3(
                        Mathf.Clamp(lineCenter + offset, -w + halfSize, w - halfSize),
                        1f,
                        h - halfSize
                    );

                    moveDir = Vector3.back;
                    waveDir = Vector3.right;
                    break;

                case SpawnSide.Bottom:
                    pos = new Vector3(
                        Mathf.Clamp(lineCenter + offset, -w + halfSize, w - halfSize),
                        1f,
                        -h + halfSize
                    );

                    moveDir = Vector3.forward;
                    waveDir = Vector3.right;
                    break;

                case SpawnSide.Right:
                    pos = new Vector3(
                        w - halfSize,
                        1f,
                        Mathf.Clamp(lineCenter + offset, -h + halfSize, h - halfSize)
                    );

                    moveDir = Vector3.left;
                    waveDir = Vector3.forward;
                    break;

                case SpawnSide.Left:
                    pos = new Vector3(
                        -w + halfSize,
                        1f,
                        Mathf.Clamp(lineCenter + offset, -h + halfSize, h - halfSize)
                    );

                    moveDir = Vector3.right;
                    waveDir = Vector3.forward;
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
                moveDir,
                waveDir
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