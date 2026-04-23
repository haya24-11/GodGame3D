using UnityEngine;
using UnityEngine.Pool;

/*<enemyのオブジェクトプール管理のスクリプト>
 * 
 * <内容>
 * ・オブジェクトプールの宣言と定義
 * ・オブジェクトプールからの取り出し
 * ・オブジェクトプールへの返還
*/
public class call_enemy : MonoBehaviour
{
    //オブジェクトプールマネージャー
    [SerializeField] GameObject enemy_straight;
    [SerializeField] GameObject boss_alpha;
    [SerializeField] GameObject enemy_accele;

    //オブジェクトプール
    ObjectPool<GameObject> enemy_straight_pool;
    ObjectPool<GameObject> boss_alpha_pool;
    ObjectPool<GameObject> enemy_accele_pool;

    void Awake()
    {
        //オブジェクトプールの定義
        enemy_straight_pool = new ObjectPool<GameObject>(
            () => Instantiate(enemy_straight),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false)
        );
        boss_alpha_pool = new ObjectPool<GameObject>(
            () => Instantiate(boss_alpha),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false)
        );
        enemy_accele_pool=new ObjectPool<GameObject>(
            ()=> Instantiate(enemy_accele),
            obj => obj.SetActive(true),
            obj => obj.SetActive(false)
        );
    }


    public GameObject method_call_enemy(int ID, Vector3 position, float angle, float rotY)
    {
        GameObject enemy;

        if (ID == 0)
            enemy = enemy_straight_pool.Get();
        else if (ID == 1)
            enemy = enemy_accele_pool.Get();
        else if (ID == 2)
            enemy = boss_alpha_pool.Get();
        else
            enemy = enemy_straight_pool.Get();

        Debug.Log("A");

        // 3D座標
        enemy.transform.position = position;

        // 3D回転（Y軸を使用）
        enemy.transform.rotation = Quaternion.Euler(0, rotY, 0);

        // 角度設定（move_enemyの中身も3D対応が必要）
        enemy_direction enemy_Direction=enemy.GetComponent<enemy_direction>();
        enemy_Direction.method_enemy_direction(angle);

        //ID情報付与(後にrelease処理をするための個体番号)
        enemy_CheckPosition enemy_CheckPosition=enemy.GetComponent<enemy_CheckPosition>();
        enemy_CheckPosition.enemy_ID = ID;

        //Release処理を呼ぶためにこのスクリプトの参照を付与
        enemy_CheckPosition.Call_Enemy = this;

        return enemy;
    }

    public void method_release_enemy(GameObject enemy_object)
    {
        //敵の種類を判別する為に参照
        enemy_CheckPosition enemy_CheckPosition=enemy_object.GetComponent<enemy_CheckPosition>();

        if(enemy_CheckPosition.enemy_ID == 0)
        {
            enemy_straight_pool.Release(enemy_object);
        }
        else if(enemy_CheckPosition.enemy_ID==1)
        {
            enemy_accele_pool.Release(enemy_object);
        }
    }


}
