using UnityEngine;

/*<Scriptの内容=enemyが画面外に出たかどうかを判定する>
 * なおenemyは画面外から出現する為座標範囲は任意とする
 * TrueならPoolに返るスクリプトを起動する
 * 
 *<Scriptの処理内容>
 *
 *
*/



public class enemy_CheckPosition : MonoBehaviour
{
    //中心座標からの距離
    [SerializeField] float distance_x;
    [SerializeField] float distance_y;

    private void Update()
    {
        //中心座標から離れている場合
        if (this.transform.position.x > distance_x || this.transform.position.x<-distance_x
            || this.transform.position.y>distance_y || this.transform.position.y<-distance_y)
        {

        }
    }
}
