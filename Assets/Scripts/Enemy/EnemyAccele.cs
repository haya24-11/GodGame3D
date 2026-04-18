// 意図：仕様通り「移動→停止→加速」を実装

using UnityEngine;

public class EnemyAccele : EnemyBase
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float accelSpeed = 4f;

    private Vector3 startPos;
    private bool stopped = false;
    private float timer = 0f;

    protected override void Start()
    {
        base.Start();
        startPos = transform.position;
    }

    void Update()
    {
        if (!stopped)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

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
                transform.Translate(Vector3.forward * accelSpeed * Time.deltaTime);
            }
        }
    }

    protected override void OnDead()
    {
        SendMessage("AddTime", 1, SendMessageOptions.DontRequireReceiver);
        base.OnDead();
    }
}