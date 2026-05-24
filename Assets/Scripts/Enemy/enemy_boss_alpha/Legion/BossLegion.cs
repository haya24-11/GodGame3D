// ============================================
// ファイル：BossLegion.cs
// 役割：分身生成・管理ボス
// 内容：
// ・中央固定
// ・Clone三角形配置
// ・Clone再生成
// ・Wave強化
// ・本体無敵
// ============================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossLegion : BossBase
{
    // ============================================
    // Clone設定
    // ============================================

    [Header("Clone設定")]

    [SerializeField]
    private GameObject clonePrefab;

    [SerializeField]
    private int spawnCount = 3;

    [SerializeField]
    private float cloneLifetime = 10f;

    [SerializeField]
    private float cloneSpeed = 3f;

    [SerializeField]
    private float triangleRadius = 3f;

    // ============================================
    // 状態
    // ============================================

    private int deadCloneCount = 0;

    private int waveLevel = 0;

    private List<GameObject> activeClones = new();

    // ============================================
    // 初期化
    // ============================================

    protected override void Start()
    {
        base.Start();

        // ========================================
        // ボスを中央固定
        // ========================================

        transform.position =
            new Vector3(0f, 1f, 0f);

        SpawnClones();
    }

    // ============================================
    // Clone生成
    // ============================================

    void SpawnClones()
    {
        activeClones.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            // ====================================
            // 三角形配置
            // ====================================

            float angle =
                (360f / spawnCount) * i;

            float rad =
                angle * Mathf.Deg2Rad;

            Vector3 pos =
                transform.position +
                new Vector3(
                    Mathf.Cos(rad) * triangleRadius,
                    0f,
                    Mathf.Sin(rad) * triangleRadius
                );

            GameObject clone =
                Instantiate(
                    clonePrefab,
                    pos,
                    Quaternion.identity
                );

            LegionClone lc =
                clone.GetComponent<LegionClone>();

            if (lc == null)
            {
                Debug.LogError(
                    "[Legion] LegionCloneが付いてない"
                );

                continue;
            }

            lc.Init(
                this,
                cloneSpeed,
                cloneLifetime,
                pos
            );

            activeClones.Add(clone);
        }

        Debug.Log(
            $"[Legion] Clone生成:{spawnCount}"
        );
    }

    // ============================================
    // Clone死亡通知
    // ============================================

    public void NotifyCloneDead()
    {
        deadCloneCount++;

        AddTime(10);

        Debug.Log(
            $"[Legion] Clone撃破:{deadCloneCount}/{spawnCount}"
        );

        // ========================================
        // 全滅
        // ========================================

        if (deadCloneCount >= spawnCount)
        {
            PowerUp();

            StartCoroutine(ReSpawnWave());
        }
    }

    // ============================================
    // 再生成
    // ============================================

    IEnumerator ReSpawnWave()
    {
        yield return new WaitForSeconds(1f);

        deadCloneCount = 0;

        SpawnClones();

        Debug.Log("[Legion] Wave再生成");
    }

    // ============================================
    // 強化
    // ============================================

    void PowerUp()
    {
        waveLevel++;

        // ========================================
        // 1回目全滅
        // ========================================

        if (waveLevel == 1)
        {
            spawnCount++;

            cloneSpeed = 4f;
        }

        // ========================================
        // 2回目全滅
        // ========================================

        else if (waveLevel >= 2)
        {
            spawnCount++;

            cloneSpeed = 6f;
        }

        Debug.Log(
            $"[Legion] 強化 Wave:{waveLevel}"
        );
    }

    // ============================================
    // 本体無敵
    // ============================================

    public override void TakeDamage(
        int damage,
        Vector3 attackerPos
    )
    {
        // 本体無敵
    }

    // ============================================
    // 被弾拡張用
    // ============================================

    protected override void OnDamaged(
        int damage,
        Vector3 attackerPos
    )
    {
    }
}