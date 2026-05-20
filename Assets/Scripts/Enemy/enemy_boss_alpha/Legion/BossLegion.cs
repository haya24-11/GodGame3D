// ============================================
// ファイル：BossLegion.cs
// 役割：分身生成・管理ボス
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
    [SerializeField] private GameObject clonePrefab;

    [SerializeField] private int spawnCount = 3;

    [SerializeField] private float cloneLifetime = 10f;

    [SerializeField] private float cloneSpeed = 3f;

    // ============================================
    // 強化状態
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

        SpawnClones();
    }

    // ============================================
    // Clone生成
    // ============================================

    void SpawnClones()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        activeClones.Clear();

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-w, w),
                1f,
                Random.Range(-h, h)
            );

            GameObject clone =
                Instantiate(clonePrefab, pos, Quaternion.identity);

            LegionClone lc =
                clone.GetComponent<LegionClone>();

            if (lc == null)
            {
                Debug.LogError("LegionCloneがPrefabに付いてない");
                continue;
            }

            lc.Init(this, cloneSpeed, cloneLifetime);

            activeClones.Add(clone);
        }

        Debug.Log($"[Legion] Clone生成:{spawnCount}");
    }

    // ============================================
    // Clone死亡通知
    // ============================================

    public void NotifyCloneDead()
    {
        deadCloneCount++;

        AddTime(10);

        if (deadCloneCount >= spawnCount)
        {
            StartCoroutine(ReSpawnWave());
        }
    }

    // ============================================
    // 再配置
    // ============================================

    IEnumerator ReSpawnWave()
    {
        yield return new WaitForSeconds(1f);

        deadCloneCount = 0;

        SpawnClones();

        Debug.Log("[Legion] Wave再生成");
    }

    // ============================================
    // 強化（全滅時）
    // ============================================

    void PowerUp()
    {
        waveLevel++;

        spawnCount++;

        cloneSpeed += (waveLevel == 1) ? 1f : 2f;

        Debug.Log($"[Legion] 強化 Wave:{waveLevel}");
    }

    // ============================================
    // Clone死亡時の通知（拡張用）
    // ============================================

    protected override void OnDamaged(int damage, Vector3 attackerPos)
    {
        // 本体は今は殴れない設計でもOK
        // ここは拡張用
    }
}