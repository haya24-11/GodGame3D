using System.Collections.Generic;
using UnityEngine;

public class SpawnBase2 : MonoBehaviour
{
    [Header("スポーンするオブジェクト")]
    public GameObject prefab;

    [Header("スポーン地点")]
    public Transform[] spawnPoints;

    [Header("スポーン間隔")]
    public float spawnInterval = 2f;

    [Header("プール数")]
    public int poolSize = 20;
    int poolCount = 0;

    protected float timer = 0f;

    // オブジェクトプール
    protected List<GameObject> pool = new List<GameObject>();

    protected virtual void Start()
    {
        CreatePool();
    }

    protected virtual void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {

            timer = 0f;
            if (poolCount < poolSize)
            {
                SpawnObject();
                poolCount++;
            }
        }
    }

    // プール生成
    protected virtual void CreatePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);

            obj.SetActive(false);

            pool.Add(obj);
        }
    }

    // 使用可能なオブジェクト取得
    protected virtual GameObject GetPoolObject()
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        // 足りなければ追加生成
        GameObject newObj = Instantiate(prefab);

        newObj.SetActive(false);

        pool.Add(newObj);

        return newObj;
    }

    protected virtual void SpawnObject()
    {
        foreach (Transform point in spawnPoints)
        {
            GameObject obj = GetPoolObject();

            obj.transform.position = point.position;
            obj.transform.rotation = point.rotation;

            obj.SetActive(true);
        }
    }
}