// 意図：HP割合で行動変化するボス

using UnityEngine;

public class EnemyBossStraight : EnemyBase
{
    private int phase = 0;

    protected override void Start()
    {
        base.Start();
    }

    void Update()
    {
        Move();

        CheckPhase();
    }

    void Move()
    {
        // 仮：直進（あとで仕様に合わせて変える）
        transform.Translate(Vector3.forward * Time.deltaTime * 2f);
    }

    void CheckPhase()
    {
        float rate = HpRate;

        if (rate <= 2f / 3f && phase == 0)
        {
            phase = 1;
            OnPhase1();
        }
        else if (rate <= 1f / 3f && phase == 1)
        {
            phase = 2;
            OnPhase2();
        }
    }

    void OnPhase1()
    {
        Debug.Log("Phase1");
        // 行動変化ここに書く
    }

    void OnPhase2()
    {
        Debug.Log("Phase2");
        // 行動変化ここに書く
    }

    protected override void OnDead()
    {
        Debug.Log("Boss Dead");
        base.OnDead();
    }
}