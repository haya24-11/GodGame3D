using UnityEngine;

/*<enemyの進行方向を決めるスクリプト>
 * <内容>
 * enemyの進む方向の数値を返すメソッド
 */

public class enemy_direction : MonoBehaviour
{
    public float direction;
    public float method_enemy_direction(float angle)
    {
        direction = angle;

        return direction;
    }
}
