// 意図：仕様通り「移動→停止→加速」を実装

using UnityEngine;

public class EnemyAccele : EnemyBase
{
    //進行方向を決めるスクリプト
    [SerializeField] enemy_direction enemy_Direction;

    //進行方向(0～360°)
    private float angle;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float accelSpeed = 4f;

    private Vector3 startPos;
    private bool stopped = false;
    private float timer = 0f;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;

        angle = enemy_Direction.direction;
    }

    void Update()
    {
        //進行方向の単位ベクトル
        Vector3 vector3 = new Vector3(
            Mathf.Cos(0) * Mathf.Cos(angle),
            Mathf.Sin(0),
            Mathf.Cos(0) * Mathf.Sin(angle)
            );

        if (!stopped)
        {
            transform.Translate(vector3 * moveSpeed * Time.deltaTime);

            if (Vector3.Distance(startPos, transform.position) >= 2f)
            {
                stopped = true;
                timer = 0f;
            }
        }
        else
        {
            timer += Time.deltaTime;

            if (timer >= 1f)
            {
                transform.Translate(vector3 * accelSpeed * Time.deltaTime);
            }
        }
    }

    protected override void OnDead()
    {
        SendMessage("AddTime", 1, SendMessageOptions.DontRequireReceiver);
        base.OnDead();
    }
}