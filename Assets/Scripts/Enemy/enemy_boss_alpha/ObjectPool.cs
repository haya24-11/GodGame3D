// ============================================
// ファイル：ObjectPool.cs
// 役割：全体共通ObjectPool
// 内容：生成済みオブジェクトを再利用する
// ============================================

using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    // ============================================
    // Singleton
    // ============================================

    public static ObjectPool Instance;

    // ============================================
    // Pool管理
    // key   : prefab
    // value : 非使用Object一覧
    // ============================================

    private Dictionary<GameObject, Queue<GameObject>>
        poolDictionary
        = new Dictionary<GameObject, Queue<GameObject>>();

    // ============================================
    // 初期化
    // ============================================

    void Awake()
    {
        // ========================================
        // Singleton化
        // ========================================

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    // ============================================
    // 取得
    // ============================================

    public GameObject Get(
        GameObject prefab,
        Vector3 position,
        Quaternion rotation
    )
    {
        // ========================================
        // Pool未生成
        // ========================================

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab]
                = new Queue<GameObject>();
        }

        Queue<GameObject> pool
            = poolDictionary[prefab];

        GameObject obj;

        // ========================================
        // Poolに残ってる
        // ========================================

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();

            if (obj == null)
            {
                obj = Instantiate(prefab);
            }
        }
        else
        {
            obj = Instantiate(prefab);
        }

        // ========================================
        // 再利用設定
        // ========================================

        obj.transform.position = position;
        obj.transform.rotation = rotation;

        obj.SetActive(true);

        return obj;
    }

    // ============================================
    // 返却
    // ============================================

    public void Return(
        GameObject prefab,
        GameObject obj
    )
    {
        if (obj == null) return;

        // ========================================
        // Pool未生成
        // ========================================

        if (!poolDictionary.ContainsKey(prefab))
        {
            poolDictionary[prefab]
                = new Queue<GameObject>();
        }

        obj.SetActive(false);

        poolDictionary[prefab]
            .Enqueue(obj);
    }
}