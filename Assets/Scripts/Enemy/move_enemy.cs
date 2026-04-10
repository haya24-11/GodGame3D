using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Rigidbody enemy_rigidbody;
    //enemyの移動速度(unit/秒)
    [SerializeField] float speed;
    //進む方向 水平角度(0～360°)
    [SerializeField] float angle_horizontal;
    //進む方向 垂直角度(-90～90°)  0=水平 90=真上 -90=真下
    [SerializeField] float angle_vertical;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemy_rigidbody = this.GetComponent<Rigidbody>();
    }


    //angleを設定する処理 水平・垂直の2軸で方向を指定
    public void tune_angle(float tune_horizontal, float tune_vertical)
    {
        angle_horizontal = tune_horizontal;
        angle_vertical = tune_vertical;
    }

    void FixedUpdate()
    {
        //水平・垂直それぞれラジアンに変換
        float radian_h = angle_horizontal * Mathf.Deg2Rad;
        float radian_v = angle_vertical * Mathf.Deg2Rad;

        //球座標→直交座標変換で3Dの単位ベクトルを作成
        //  X = cos(垂直) * cos(水平)
        //  Y = sin(垂直)            ← Unityは上方向がY軸
        //  Z = cos(垂直) * sin(水平)
        Vector3 vector3 = new Vector3(
            Mathf.Cos(radian_v) * Mathf.Cos(radian_h),
            Mathf.Sin(radian_v),
            Mathf.Cos(radian_v) * Mathf.Sin(radian_h)
        );

        //単位ベクトル×速度をlinearVelocityに適用
        enemy_rigidbody.linearVelocity = vector3 * speed;
    }
}
