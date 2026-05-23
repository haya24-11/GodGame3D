// ============================================
// ファイル：ObjectPool.cs
// 役割：汎用オブジェクトプール
// ============================================

using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    // ============================================
    // シングルトン
    // ============================================

    public static ObjectPool Instance;

    private void Awake()
    {
        Instance = this;
    }

    // ============================================
    // プール本体
    // ============================================

    private Dictionary<GameObject, Queue<GameObject>> pools
        = new();

    // ============================================
    // 生成
    // ============================================

    public GameObject Get(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Queue<GameObject>();
        }

        GameObject obj;

        if (pools[prefab].Count > 0)
        {
            obj = pools[prefab].Dequeue();
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab);
        }

        obj.transform.position = pos;
        obj.transform.rotation = rot;

        return obj;
    }

    // ============================================
    // 返却
    // ============================================

    public void Return(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);

        if (!pools.ContainsKey(prefab))
        {
            pools[prefab] = new Queue<GameObject>();
        }

        pools[prefab].Enqueue(obj);
    }
}