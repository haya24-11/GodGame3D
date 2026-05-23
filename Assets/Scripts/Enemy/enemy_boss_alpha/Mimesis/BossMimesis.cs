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
        // ========================================
        // 最終フェーズ
        // ========================================

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

        aliveSlimeCount = 3;

        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-w, w),
                1f,
                Random.Range(-h, h)
            );

            GameObject obj =
                ObjectPool.Instance.Get(
                    slimePrefab,
                    pos,
                    Quaternion.identity
                );

            MimesisSlime slime =
                obj.GetComponent<MimesisSlime>();

            slime.Init(this, slimePrefab, 8f);
        }

        // 全破壊待ち
        while (aliveSlimeCount > 0)
        {
            yield return null;
        }

        TakeBossDamage(10);

        yield return new WaitForSeconds(1f);

        StartNextAction();
    }

    // ============================================
    // Slime死亡通知
    // ============================================

    public void NotifySlimeDead()
    {
        aliveSlimeCount--;
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
        transform.Translate(
            chargeDir * chargeSpeed * Time.deltaTime,
            Space.World
        );

        Camera cam = Camera.main;

        float h = cam.orthographicSize;

        if (transform.position.z < -h - 3f)
        {
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
        if (state == State.Charge)
        {
            TakeDamage(10, transform.position);

            StartCoroutine(NextActionDelay());
        }

        if (state == State.Area)
        {
            TakeDamage(10, transform.position);
        }
    }
}