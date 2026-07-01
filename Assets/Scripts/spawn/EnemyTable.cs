using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ========================================
// 敵1体分の出現座標データ
// ========================================
[System.Serializable]
public class EnemySpawnPoint
{
    [Tooltip("出現座標 (右図座標系に基づく / Z座標対応)")]
    public Vector3 spawnPosition;
}

// ========================================
// 敵の出現パターン1セット分のデータ
// 例: 1陣、2陣、ボス陣 など
// ========================================
[System.Serializable]
public class EnemyWaveData
{
    [Header("--- 基本設定 ---")]

    [Tooltip("この陣の名前 (Inspector上の識別用 例: 1陣, Boss陣)")]
    public string waveName = "Wave";

    [Tooltip("出現する敵のPrefab種類")]
    public GameObject enemyPrefab;

    [Tooltip("出現時間 (ステージ開始を0秒として秒単位)")]
    public float spawnTime = 0f;

    [Tooltip("向き: 度数法で0=真右方向, 90=真上方向 (敵オブジェクト自体の回転角度)")]
    public float direction = 180f;

    [Tooltip("出現座標リスト (リストに追加した数が出現数になる)")]
    public List<EnemySpawnPoint> spawnPoints = new List<EnemySpawnPoint>();

    /// <summary>出現数 (spawnPointsの件数を自動返却)</summary>
    public int SpawnCount => spawnPoints != null ? spawnPoints.Count : 0;

    [Header("--- 消滅・プール設定 ---")]

    [Tooltip("消滅時間 (出現してから何秒後にプールへ戻すか)")]
    public float despawnTime = 5f;

 
}

// ========================================
// EnemyTable
// 機能: 敵の出現パターンを管理するオブジェクト
//       出現パターンは「敵種類、タイミング、座標、画面への入射角、Rotation、敵速度」で決定
//       ボス含む全ての敵はこれに沿って出現する
// アタッチ先: EnemySpawner (Manager オブジェクト)
// ========================================
public class EnemyTable : MonoBehaviour
{
    [Header("========== 敵出現パターン一覧 ==========")]
    [Tooltip("全陣のデータをここに登録する。ボス含む全ての敵はこのテーブルに沿って出現する。")]
    public EnemyWaveData[] waveDataArray;

    // ========================================
    // オブジェクトプール本体
    // key: prefabの参照, value: 待機中のオブジェクトキュー
    // ========================================
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary
        = new Dictionary<GameObject, Queue<GameObject>>();

    [Header("========== プール設定 ==========")]
    [Tooltip("各Prefabにつき初期生成しておくオブジェクト数")]
    public int initialPoolSize = 5;

    // ========================================
    // 初期化: 全Prefabのプールを事前生成
    // ========================================
    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        if (waveDataArray == null) return;

        foreach (var wave in waveDataArray)
        {
            if (wave.enemyPrefab == null) continue;
            if (poolDictionary.ContainsKey(wave.enemyPrefab)) continue;

            Queue<GameObject> queue = new Queue<GameObject>();
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = CreateNewObject(wave.enemyPrefab);
                queue.Enqueue(obj);
            }
            poolDictionary.Add(wave.enemyPrefab, queue);
        }
    }

    // ========================================
    // プール用オブジェクト新規生成 (非アクティブ状態で生成)
    // ========================================
    private GameObject CreateNewObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        return obj;
    }

    // ========================================
    // プールからオブジェクトを取得
    // ========================================
    public GameObject GetFromPool(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            // 初回登録されていない場合は新規プール作成
            Queue<GameObject> newQueue = new Queue<GameObject>();
            poolDictionary.Add(prefab, newQueue);
        }

        Queue<GameObject> queue = poolDictionary[prefab];

        if (queue.Count > 0)
        {
            GameObject obj = queue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            // プールが空の場合は新規生成して返す
            GameObject obj = CreateNewObject(prefab);
            obj.SetActive(true);
            return obj;
        }
    }

    // ========================================
    // オブジェクトをプールへ返却
    // ========================================
    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        if (!poolDictionary.ContainsKey(prefab))
        {
            Queue<GameObject> newQueue = new Queue<GameObject>();
            poolDictionary.Add(prefab, newQueue);
        }

        obj.SetActive(false);
        obj.transform.SetParent(this.transform);
        poolDictionary[prefab].Enqueue(obj);
    }

    // ========================================
    // 指定時間後にプールへ返却するコルーチン呼び出し
    // ========================================
    public void ReturnToPoolAfterDelay(GameObject prefab, GameObject obj, float delay)
    {
        StartCoroutine(ReturnToPoolCoroutine(prefab, obj, delay));
    }

    private IEnumerator ReturnToPoolCoroutine(GameObject prefab, GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (obj != null && obj.activeSelf)
        {
            ReturnToPool(prefab, obj);
        }
    }
}
