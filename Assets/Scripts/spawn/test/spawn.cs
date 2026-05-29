using UnityEngine;

public class SpawnBase : MonoBehaviour
{
    [Header("スポーンするオブジェクト")]
    public GameObject prefab;

    [Header("スポーン地点")]
    public Transform[] spawnPoints;

    [Header("スポーン間隔")]
    public float spawnInterval = 2f;

    [Header("最大スポーン数")]
    public int spawnmaxcnt = 5;
    //スポーンした回数
    int spawncount = 0;
    protected float timer = 0f;

    protected virtual void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            if (spawncount < spawnmaxcnt)
            {
                SpawnObject();
                spawncount++;
                Debug.Log("スポーンしました" 
                   + "\n スポーンオブジェクト：" + prefab
                     + "\n　スポーン回数：" + spawncount);
            }
        }
    }

    protected virtual void SpawnObject()
    {
        foreach (Transform point in spawnPoints)
        {
            Instantiate(
                prefab,
                point.position,
                point.rotation
            );
        }
    }
}