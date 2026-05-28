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

    [Header("Area")]
    [SerializeField] private GameObject blueWarningPrefab;
    [SerializeField] private float areaWarningTime = 1f;
    [SerializeField] private float areaStayTimePhase2 = 1f;

    private GameObject activeAreaWarning;

    [Header("Final")]
    [SerializeField] private float finalEdgeMargin = 3f;
    [SerializeField] private float finalOrbitSpeed = 90f;

    private float finalAngle = 0f;

    // ============================================
    // Charge
    // ============================================

    [Header("Charge")]

    [SerializeField]
    private float chargeSpeed = 12f;

    private Vector3 chargeDir;

    private bool chargeHit = false;

    [SerializeField]
    private GameObject redWarningPrefab;

    [SerializeField]
    private float chargeWarningTime = 2f;

    [SerializeField]
    private float chargeSpeedPhase2 = 15f;

    private GameObject activeWarning;

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

            case State.Final:
                UpdateFinal();
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
        StartCoroutine(ChargeRoutine());
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
        float w = h * cam.aspect;

        if (Mathf.Abs(transform.position.x) > w + 3f ||
            Mathf.Abs(transform.position.z) > h + 3f)
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
            Rect areaRect = GetRandomAreaRect();

            Vector3 warningPos = new Vector3(
                areaRect.center.x,
                0.05f,
                areaRect.center.y
            );

            // =========================
            // 青予告表示
            // =========================

            if (blueWarningPrefab != null)
            {
                activeAreaWarning = Instantiate(
                    blueWarningPrefab,
                    warningPos,
                    Quaternion.identity
                );

                activeAreaWarning.transform.localScale =
                    new Vector3(areaRect.width, 1f, areaRect.height);
            }

            yield return new WaitForSeconds(areaWarningTime);

            if (activeAreaWarning != null)
            {
                Destroy(activeAreaWarning);
            }

            // =========================
            // 予告範囲内に出現
            // =========================

            transform.position = new Vector3(
                Random.Range(areaRect.xMin, areaRect.xMax),
                1f,
                Random.Range(areaRect.yMin, areaRect.yMax)
            );

            float stayTime =
                totalDamage >= 30
                ? areaStayTimePhase2
                : areaStayTime;

            float timer = 0f;

            while (timer < stayTime)
            {
                if (state != State.Area)
                {
                    yield break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            areaLoopCount++;
        }

        state = State.Idle;

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

        isActionChanging = true;

        finalAngle = 0f;

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

    IEnumerator ChargeRoutine()
    {
        state = State.Idle;
        isActionChanging = true;

        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        int side = Random.Range(0, 4);

        Vector3 startPos = Vector3.zero;
        Vector3 dir = Vector3.zero;
        Vector3 warningPos = Vector3.zero;

        switch (side)
        {
            case 0:
                startPos = new Vector3(Random.Range(-w, w), 1f, h + 2f);
                dir = Vector3.back;
                warningPos = new Vector3(startPos.x, 0.05f, 0f);
                break;

            case 1:
                startPos = new Vector3(Random.Range(-w, w), 1f, -h - 2f);
                dir = Vector3.forward;
                warningPos = new Vector3(startPos.x, 0.05f, 0f);
                break;

            case 2:
                startPos = new Vector3(w + 2f, 1f, Random.Range(-h, h));
                dir = Vector3.left;
                warningPos = new Vector3(0f, 0.05f, startPos.z);
                break;

            case 3:
                startPos = new Vector3(-w - 2f, 1f, Random.Range(-h, h));
                dir = Vector3.right;
                warningPos = new Vector3(0f, 0.05f, startPos.z);
                break;
        }

        float warningTime =
            totalDamage >= 30
            ? 1f
            : chargeWarningTime;

        if (redWarningPrefab != null)
        {
            activeWarning = Instantiate(
                redWarningPrefab,
                warningPos,
                Quaternion.identity
            );
        }

        yield return new WaitForSeconds(warningTime);

        if (activeWarning != null)
        {
            Destroy(activeWarning);
        }

        transform.position = startPos;
        chargeDir = dir;

        chargeSpeed =
            totalDamage >= 30
            ? chargeSpeedPhase2
            : 12f;

        chargeHit = false;
        isActionChanging = false;
        state = State.Charge;
    }

    Rect GetRandomAreaRect()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        int areaIndex = Random.Range(0, 4);

        bool right = areaIndex % 2 == 1;
        bool top = areaIndex >= 2;

        float xMin = right ? 0f : -w;
        float xMax = right ? w : 0f;

        float zMin = top ? 0f : -h;
        float zMax = top ? h : 0f;

        return Rect.MinMaxRect(
            xMin,
            zMin,
            xMax,
            zMax
        );
    }

    void UpdateFinal()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        float radiusX = Mathf.Max(0f, w - finalEdgeMargin);
        float radiusZ = Mathf.Max(0f, h - finalEdgeMargin);

        finalAngle += finalOrbitSpeed * Time.deltaTime;

        float rad = finalAngle * Mathf.Deg2Rad;

        Vector3 pos = new Vector3(
            Mathf.Cos(rad) * radiusX,
            1f,
            Mathf.Sin(rad) * radiusZ
        );

        transform.position = pos;
    }
}