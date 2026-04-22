using UnityEngine;



// 敵の出現パターンのテーブルを制作するためのスクリプト]


[CreateAssetMenu(fileName = "enemy_SpawnTable", menuName = "Scriptable Objects/enemy_SpawnTable")]
// ScriptableObject＝プロジェクトに属するデータ　シーン間で共有される
//　　　　　　　　　→その場で実行されるインスタンスである通常のスクリプトと異なり　共有のデータとして扱われるスクリプト


public class enemy_SpawnTable : ScriptableObject
{

    // データ群のシリアライズ化
    [System.Serializable]
    public class EnemySpawnData
    {
        // 出現タイミング　spawnTime=スクロール速度×秒であるため、Timeで取ってはいない
        public float spawnTime;
        // 出現座標
        public Vector3 spawnPosition;
        // 入射角
        public float spawnAngle;
        // 回転
        public float spawnRoat;
        // どの敵か
        public int enemyType;
    }

    public EnemySpawnData[] spawnTable;


}
