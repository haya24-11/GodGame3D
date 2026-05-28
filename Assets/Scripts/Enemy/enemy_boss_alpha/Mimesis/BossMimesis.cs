// ============================================
// ファイル：BossMimesis.cs
// 意図：最終ボス
// 意図：複数行動切替型
// ============================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossMimesis : BossBase
{
    // ============================================
    // State
    // ============================================

    private enum State
    {
        Idle,
        Slime,
        Charge,
        Area,
        Final
    }

    private State state;

    // ============================================
    // Slime
    // ============================================

    [Header("Slime")]

    [SerializeField]
    private GameObject slimePrefab;

    private int aliveSlimeCount = 0;

    private bool isActionChanging = false;

    private enum SpawnSide
    {
        Top,
        Bottom,
        Right,
        Left
    }

    private enum SpawnLineType
    {
        Center,
        CornerA,
        CornerB
    }

    [SerializeField]
    private float slimeSpeed = 8f;

    [SerializeField]
    private float slimeSize = 1f;

    [SerializeField]
    private int slimeSpawnCount = 3;

    private bool slimeActionFinished = false;

    // ============================================
    // Charge
    // ============================================

    [Header("Charge")]

    [SerializeField]
    private float chargeSpeed = 12f;

    private Vector3 chargeDir;

    private bool chargeHit = false;

    // ============================================
    // Area
    // ============================================

    private int areaLoopCount = 0;

    private float areaStayTime = 2f;

    // ============================================
    // ダメージ蓄積
    // ============================================

    private int totalDamage = 0;

    // ============================================
    // 初期化
    // ============================================

    protected override void Start()
    {
        base.Start();

        StartNextAction();
    }

    // ============================================
    // 更新
    // ============================================

    void Update()
    {
        switch (state)
        {
            case State.Charge:
                UpdateCharge();
                break;
        }
    }

    // ============================================
    // 次行動
    // ============================================

    void StartNextAction()
    {
        isActionChanging = false;

        if (totalDamage >= 60)
        {
            EnterFinalPhase();
            return;
        }

        int rand = Random.Range(0, 3);

        switch (rand)
        {
            case 0:
                StartCoroutine(SlimeRoutine());
                break;

            case 1:
                StartCharge();
                break;

            case 2:
                StartCoroutine(AreaRoutine());
                break;
        }
    }

    // ============================================
    // Slime行動
    // ============================================

    IEnumerator SlimeRoutine()
    {
        state = State.Slime;

        slimeActionFinished = false;
        aliveSlimeCount = slimeSpawnCount;

        SpawnSlimeFormation();

        while (!slimeActionFinished)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        StartNextAction();
    }

    // ============================================
    // Slime死亡通知
    // ============================================

    public void NotifySlimeDead()
    {
        if (state != State.Slime) return;
        if (slimeActionFinished) return;

        aliveSlimeCount--;

        if (aliveSlimeCount <= 0)
        {
            slimeActionFinished = true;

            TakeDamage(10, transform.position);
        }
    }

    public void NotifySlimeOutScreen()
    {
        if (state != State.Slime) return;
        if (slimeActionFinished) return;

        slimeActionFinished = true;
    }

    // ============================================
    // Charge開始
    // ============================================

    void StartCharge()
    {
        state = State.Charge;

        chargeHit = false;

        Camera cam = Camera.main;

        float h = cam.orthographicSize;

        transform.position =
            new Vector3(0f, 1f, h + 2f);

        chargeDir = Vector3.back;
    }

    // ============================================
    // Charge更新
    // ============================================
    void UpdateCharge()
    {
        if (isActionChanging) return;

        transform.Translate(
            chargeDir * chargeSpeed * Time.deltaTime,
            Space.World
        );

        Camera cam = Camera.main;

        float h = cam.orthographicSize;

        if (transform.position.z < -h - 3f)
        {
            isActionChanging = true;
            state = State.Idle;

            StartCoroutine(NextActionDelay());
        }
    }

    // ============================================
    // Area行動
    // ============================================

    IEnumerator AreaRoutine()
    {
        state = State.Area;

        areaLoopCount = 0;

        while (areaLoopCount < 3)
        {
            Vector3 pos = GetRandomAreaPosition();

            transform.position = pos;

            float timer = 0f;

            while (timer < areaStayTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            areaLoopCount++;
        }

        yield return new WaitForSeconds(1f);

        StartNextAction();
    }

    // ============================================
    // ランダム位置
    // ============================================

    Vector3 GetRandomAreaPosition()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        return new Vector3(
            Random.Range(-w, w),
            1f,
            Random.Range(-h, h)
        );
    }

    // ============================================
    // ボスダメージ
    // ============================================

    void TakeBossDamage(int damage)
    {
        totalDamage += damage;

        currentHp -= damage;

        Debug.Log(
            $"[Mimesis] Damage:{damage}"
        );

        if (currentHp <= 0)
        {
            Die();
        }
    }

    // ============================================
    // 最終フェーズ
    // ============================================

    void EnterFinalPhase()
    {
        state = State.Final;

        Debug.Log("[Mimesis] FINAL PHASE");
    }

    // ============================================
    // 次行動待機
    // ============================================

    IEnumerator NextActionDelay()
    {
        yield return new WaitForSeconds(1f);

        StartNextAction();
    }

    // ============================================
    // 被弾
    // ============================================

    protected override void OnDamaged(
        int damage,
        Vector3 attackerPos
    )
    {
        if (isActionChanging) return;

        if (state == State.Charge ||
            state == State.Area)
        {
            isActionChanging = true;
            state = State.Idle;

            StartCoroutine(NextActionDelay());
        }
    }
    public override void TakeDamage(
    int damage,
    Vector3 attackerPos
)
    {
        bool canDamage = false;

        switch (state)
        {
            case State.Charge:
            case State.Area:
            case State.Final:

                canDamage = true;

                break;
        }

        if (!canDamage)
        {
            return;
        }

        totalDamage += damage;

        base.TakeDamage(
            damage,
            attackerPos
        );
    }

    void SpawnSlimeFormation()
    {
        if (slimePrefab == null)
        {
            Debug.LogError("[Mimesis] slimePrefab 未設定");
            return;
        }

        if (ObjectPool.Instance == null)
        {
            Debug.LogError("[Mimesis] ObjectPool.Instance が存在しない");
            return;
        }

        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        SpawnSide side = (SpawnSide)Random.Range(0, 4);

        float spacing = slimeSize * 2f;

        float totalLength = spacing * (slimeSpawnCount - 1);
        float startOffset = -totalLength * 0.5f;

        SpawnLineType lineType =
            (SpawnLineType)Random.Range(0, 3);

        float lineCenter = 0f;

        switch (lineType)
        {
            case SpawnLineType.Center:
                lineCenter = 0f;
                break;

            case SpawnLineType.CornerA:
                lineCenter = -Mathf.Min(w, h) * 0.5f;
                break;

            case SpawnLineType.CornerB:
                lineCenter = Mathf.Min(w, h) * 0.5f;
                break;
        }

        float halfSize = slimeSize * 0.5f;

        Vector3 moveDir = Vector3.zero;

        MimesisSlime.MoveType formationMoveType =
            Random.value < 0.5f
            ? MimesisSlime.MoveType.Straight
            : MimesisSlime.MoveType.Wave;

        for (int i = 0; i < slimeSpawnCount; i++)
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
                slimePrefab,
                pos,
                Quaternion.identity
            );

            MimesisSlime slime =
                obj.GetComponent<MimesisSlime>();

            if (slime == null)
            {
                Debug.LogError("[Mimesis] MimesisSlime がPrefabに付いてない");
                return;
            }

            slime.Init(
                this,
                slimePrefab,
                slimeSpeed,
                moveDir,
                waveDir,
                formationMoveType
            );
        }

        Debug.Log("[Mimesis] Slime隊列生成");
    }
}