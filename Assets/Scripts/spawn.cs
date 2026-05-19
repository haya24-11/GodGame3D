using UnityEngine;

public class spown : MonoBehaviour
{

    [Header("スポーンするオブジェクト")]
    public GameObject prefab;

    [Header("スポーン座標")]
    public float spawnX = 0f;
    public float spawnY = 0f;
    public float spawnZ = 0f;

    [Header("スポーン間隔")]
    public float spawnInterval = 2f;

    [Header("最大スポーン数")]
    public int maxSpawnCount = 10;

    private float timer = 0f;
    private int currentSpawnCount = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            if (currentSpawnCount < maxSpawnCount)
            {
                SpawnObject();//
            }
        }
    }

    void SpawnObject()
    {
        //XYZ座標を事前に決めた場所にする
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, spawnZ);
        //ポジションでスポーン
        Instantiate(prefab, spawnPosition, Quaternion.identity);

        Debug.Log("スポーンしました: " + obj.name);

        currentSpawnCount++;
    }
}
