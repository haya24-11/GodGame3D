using System.Collections.Generic;
using UnityEngine;

//スポーンテーブルを基に敵を出現させるスクリプト
//※敵はオブジェクトプールから呼び出す

public class enemy_playback : MonoBehaviour
{
    // この変数にスクロール速度を毎秒足す
    float counter;

    //敵を生成するスクリプト
    [SerializeField] private call_enemy call_enemy_script;
    // 敵の種類ごとのプレハブ
    [SerializeField] private GameObject[] enemyTypeTable;
    // 敵の出現テーブル
    [SerializeField] private enemy_SpawnTable[] spawnTableSO;

    // ソート済みテーブル
    private List<enemy_SpawnTable.EnemySpawnData> sortedSpawnTable;
    private int currentSpawnIndex = 0;

    //敵の出現テーブルが全て既読で、敵がいないときにtrueにする
    public static bool isNoEnemy = false;

    //データ群のシリアライズ化
    [System.Serializable]
    public class EnemyTypeData
    {
        //敵の種類
        public GameObject enemyPrefab;
        //敵の名前
        public string enemyName;
        //倒したら増える制限時間
        public int enemyScore;
    }

    private GameObject[] initialEnemyTypeTable;
    private enemy_SpawnTable[] initialSpawnTableSO;
    private float initialGameTimer;
    private List<enemy_SpawnTable.EnemySpawnData> initialSortedSpawnTable;
    private int initialCurrentSpawnIndex;



    void Awake()
    {
        SaveInitialValues();
    }
    void SaveInitialValues()
    {
        initialEnemyTypeTable = enemyTypeTable;
        initialSpawnTableSO = spawnTableSO;

        initialGameTimer = counter;
        initialSortedSpawnTable = sortedSpawnTable;
        initialCurrentSpawnIndex = currentSpawnIndex;

    }
    void ResetToInitialValues()
    {
        enemyTypeTable = initialEnemyTypeTable;
        spawnTableSO = initialSpawnTableSO;
        counter = initialGameTimer;
        sortedSpawnTable = initialSortedSpawnTable;
        currentSpawnIndex = initialCurrentSpawnIndex;
        isNoEnemy = false;
    }

    public void Initialize()
    {
        ResetToInitialValues();

        // spawnTableSOのnullチェック
        if (spawnTableSO == null || spawnTableSO.Length == 0)
        {
            Debug.LogError("spawnTableSO が設定されていません。");
            return;
        }

        //配列にわかれたテーブルを一つにまとめる
        List<enemy_SpawnTable.EnemySpawnData> allSpawnTable = new List<enemy_SpawnTable.EnemySpawnData>();
        foreach (var table in spawnTableSO)
        {
            if (table == null)
            {
                Debug.LogError("spawnTableSOの要素にnullが含まれています。");
                continue;
            }

            foreach (var spawnData in table.spawnTable)
            {
                allSpawnTable.Add(spawnData);
            }
        }

        // まとめたスポーンテーブルを時間順にソート
        sortedSpawnTable = new List<enemy_SpawnTable.EnemySpawnData>(allSpawnTable);
        sortedSpawnTable.Sort((a, b) => a.spawnTime.CompareTo(b.spawnTime));

    }
    void Start()
    {
        Initialize();
    }

    void Update()
    {
        //　便宜上秒でcounterを増やしているが　実際にはスクロール速度をかける
        counter += Time.deltaTime;

        //currentSpawnIndex=次に出す敵の要素番号
        if (currentSpawnIndex < sortedSpawnTable.Count && counter >= sortedSpawnTable[currentSpawnIndex].spawnTime)
        {
            SpawnEnemy(sortedSpawnTable[currentSpawnIndex]);
            currentSpawnIndex++;
        }

    }
    private void SpawnEnemy(enemy_SpawnTable.EnemySpawnData data)
    {
        // 配列の境界チェック
        if (data.enemyType < 0 || data.enemyType >= enemyTypeTable.Length)
        {
            Debug.LogError($"Invalid enemy type: {data.enemyType}. Array size: {enemyTypeTable.Length}");
            return;
        }

        // 敵プレハブのIDを基に生成
        call_enemy_script.method_call_enemy(data.enemyType, data.spawnPosition, data.spawnAngle, data.spawnRoat);
    }
}
