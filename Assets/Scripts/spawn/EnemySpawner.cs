using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ========================================
// EnemySpawner
// 機能: EnemyTableのwaveDataArrayを参照し、
//       指定タイミングで敵をスポーンする
// アタッチ先: EnemyTable と同じ Manager オブジェクト
// ========================================
[RequireComponent(typeof(EnemyTable))]
public class EnemySpawner : MonoBehaviour
{
    // EnemyTableへの参照
    private EnemyTable enemyTable;

    [Header("========== スポナー設定 ==========")]
    [Tooltip("ステージ開始と同時に自動でスポーンを開始するか")]
    public bool autoStart = true;

    [Tooltip("背景スクロール速度 (演出参考値 / 必要に応じて敵速度補正に使用)")]
    public float backgroundScrollSpeed = 2f;

    // ステージ開始からの経過時間
    private float stageTimer = 0f;

    // 各Waveがすでにスポーン済みかのフラグ
    private bool[] waveSpawned;

    private bool isRunning = false;

    // ========================================
    // 初期化
    // ========================================
    private void Awake()
    {
        enemyTable = GetComponent<EnemyTable>();
    }

    private void Start()
    {
        if (autoStart)
            StartStage();
    }

    // ========================================
    // ステージ開始 (外部からも呼び出し可能)
    // ========================================
    public void StartStage()
    {
        stageTimer = 0f;
        isRunning = true;

        if (enemyTable.waveDataArray != null)
            waveSpawned = new bool[enemyTable.waveDataArray.Length];

        Debug.Log("[EnemySpawner] Stage Started.");
    }

    // ========================================
    // ステージ停止
    // ========================================
    public void StopStage()
    {
        isRunning = false;
        Debug.Log("[EnemySpawner] Stage Stopped.");
    }

    // ========================================
    // ステージタイマー更新 & スポーントリガー
    // ========================================
    private void Update()
    {
        if (!isRunning) return;
        if (enemyTable.waveDataArray == null) return;

        stageTimer += Time.deltaTime;

        for (int i = 0; i < enemyTable.waveDataArray.Length; i++)
        {
            if (waveSpawned[i]) continue;

            EnemyWaveData wave = enemyTable.waveDataArray[i];

            if (stageTimer >= wave.spawnTime)
            {
                SpawnWave(wave);
                waveSpawned[i] = true;
            }
        }
    }

    // ========================================
    // 1陣分の敵をまとめてスポーン
    // ========================================
    private void SpawnWave(EnemyWaveData wave)
    {
        if (wave.enemyPrefab == null)
        {
            Debug.LogWarning($"[EnemySpawner] Wave '{wave.waveName}' : enemyPrefab が未設定です。");
            return;
        }

        int count = Mathf.Min(wave.spawnCount, wave.spawnPoints.Count);

        if (wave.spawnPoints.Count < wave.spawnCount)
        {
            Debug.LogWarning($"[EnemySpawner] Wave '{wave.waveName}' : " +
                             $"spawnCount({wave.spawnCount}) > spawnPoints数({wave.spawnPoints.Count}) " +
                             $"のため {wave.spawnPoints.Count} 体のみスポーンします。");
        }

        for (int i = 0; i < count; i++)
        {
            SpawnEnemy(wave, i);
        }

        Debug.Log($"[EnemySpawner] Wave '{wave.waveName}' スポーン完了 ({count}体) / time:{stageTimer:F2}s");
    }

    // ========================================
    // 敵1体をスポーン
    // ========================================
    private void SpawnEnemy(EnemyWaveData wave, int index)
    {
        GameObject obj = enemyTable.GetFromPool(wave.enemyPrefab);

        // 座標設定 (Vector3 / X・Y・Z すべて反映)
        obj.transform.position = wave.spawnPoints[index].spawnPosition;

        // 向き設定
        // 度数法: Y軸回転　0の場合だと左向き 90だと奥に動く
        // Y軸回転に適用
        obj.transform.rotation = Quaternion.Euler(0f, wave.direction, 0f);

        // 移動速度・スポーン情報を敵本体へ渡す (IEnemyInitializable 実装があれば)
        IEnemyInitializable initTarget = obj.GetComponent<IEnemyInitializable>();
        if (initTarget != null)
        {
            initTarget.Initialize(wave.direction, wave.moveSpeed);
        }

        // 消滅タイマー開始 (despawnTime 後にプールへ返却)
        enemyTable.ReturnToPoolAfterDelay(wave.enemyPrefab, obj, wave.despawnTime);
    }

    // ========================================
    // 外部から任意のタイミングで敵を手動スポーン
    // ========================================
    public void SpawnManual(EnemyWaveData wave)
    {
        SpawnWave(wave);
    }
}

// ========================================
// IEnemyInitializable
// 敵側スクリプトに実装することで、
// スポーン時に向きと速度を受け取れる
// ========================================
public interface IEnemyInitializable
{
    /// <summary>
    /// スポーン時に呼ばれる初期化メソッド
    /// </summary>
    /// <param name="direction">向き (度数法: 0=真左, 90=奥)</param>
    /// <param name="moveSpeed">移動速度</param>
    void Initialize(float direction, float moveSpeed);
}
