using UnityEngine;
using UnityEngine.Pool;
public class call_enemy : MonoBehaviour
{
    //オブジェクトプールマネージャー
    [SerializeField] GameObject Enemy;
    [SerializeField] GameObject Boss;

    //オブジェクトプール
    ObjectPool<GameObject> enemy_pool;
    ObjectPool<GameObject> boss_pool;

    void Awake()
    {
        //オブジェクトプールの定義
        enemy_pool = new ObjectPool<GameObject>(
            () => Instantiate(Enemy),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false)
        );

        boss_pool = new ObjectPool<GameObject>(
            () => Instantiate(Boss),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false)
        );
    }


    public GameObject method_call_enemy(int ID, Vector3 position, float angle, float rotY)
    {
        GameObject enemy;

        if (ID == 0)
            enemy = enemy_pool.Get();
        else if (ID == 1)
            enemy = boss_pool.Get();
        else
            enemy = enemy_pool.Get();

        // 3D座標
        enemy.transform.position = position;

        // 3D回転（Y軸を使用）
        enemy.transform.rotation = Quaternion.Euler(0, rotY, 0);

        // 角度設定（move_enemyの中身も3D対応が必要）
        MoveEnemy move_Enemy = enemy.GetComponent<MoveEnemy>();
        move_Enemy.tune_angle(angle, rotY);

        return enemy;
    }




}
