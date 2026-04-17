// どのファイルのどこを変更：EnemyBossStraight.cs（新規）
// 意図：状態管理ベースを構築

using UnityEngine;

public class EnemyBossStraight : EnemyBase
{
    private enum State
    {
        Spawn,
        MoveIn,
        Stop,
        Wait,
        Feint,
        Charge,
        Exit
    }

    private State state;

    [Header("速度")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chargeSpeed = 5f;

    private float timer;
    private Vector3 moveDir;
    private Vector3 startPos;
    private int phase = 0;

    // 意図：フェイント制御
    private bool canFeint = false;     // Phase1でON
    private bool skipNextFeint = false; // 連続防止

    protected override void Start()
    {
        base.Start();
        state = State.Spawn;
        Respawn();
    }

    void Update()
    {
        switch (state)
        {
            case State.MoveIn:
                MoveIn();
                break;

            case State.Stop:
                Stop();
                break;

            case State.Wait:
                Wait();
                break;

            case State.Charge:
                Charge();
                break;

            case State.Exit:
                Exit();
                break;

            case State.Feint:
                Feint();
                break;
        }
    }

    void Respawn()
    {
        var cam = Camera.main;
        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        // 上か下から出す（縦移動のみ）
        bool fromTop = Random.value > 0.5f;

        float x = Random.Range(-w + 1f, w - 1f);
        float z = fromTop ? h + 1f : -h - 1f;

        transform.position = new Vector3(x, 1f, z);

        moveDir = fromTop ? Vector3.back : Vector3.forward;

        startPos = transform.position;

        state = State.MoveIn;

        startPos = transform.position;
    }

    void MoveIn()
    {
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);

        if (Vector3.Distance(startPos, transform.position) >= 0.5f)
        {
            state = State.Stop;
        }
    }

    void Stop()
    {
        timer = 0f;
        state = State.Wait;
    }

    void Wait()
    {
        timer += Time.deltaTime;

        // 意図：フェイント分岐
        if (timer >= 1f)
        {
            // フェイント条件
            if (canFeint && !skipNextFeint && Random.value < 0.75f)
            {
                feintStart = transform.position;

                state = State.Feint;
                skipNextFeint = true; // 次は封印
            }
            else
            {
                state = State.Charge;
                skipNextFeint = false;
            }
        }
    }

    void Charge()
    {
        transform.Translate(moveDir * chargeSpeed * Time.deltaTime);

        // 画面外判定
        var cam = Camera.main;
        float h = cam.orthographicSize;

        if (Mathf.Abs(transform.position.z) > h + 2f)
        {
            state = State.Exit;
        }
    }

    void Exit()
    {
        Respawn();
    }

    public override void TakeDamage(int damage)
    {
        float before = HpRate;

        base.TakeDamage(damage);

        float after = HpRate;

        // 2/3
        if (before > 2f / 3f && after <= 2f / 3f)
        {
            Phase1();
        }
        // 1/3
        else if (before > 1f / 3f && after <= 1f / 3f)
        {
            Phase2();
        }
    }

    void Phase1()
    {
        Debug.Log("Phase1");

        moveSpeed = 3f;
        canFeint = true;

        // 吹っ飛び（簡易）
        state = State.Charge;
        chargeSpeed = 8f;
    }

    void Phase2()
    {
        Debug.Log("Phase2");

        moveSpeed = 4f;
        chargeSpeed = 7f;

        state = State.Charge;
    }

    // 意図：逆方向に0.5unit戻る
    private Vector3 feintStart;
    private float feintDistance = 0.5f;

    void FeintStart()
    {
        feintStart = transform.position;
    }

    void Feint()
    {
        // 逆方向に移動
        transform.Translate(-moveDir * moveSpeed * Time.deltaTime);

        if (Vector3.Distance(feintStart, transform.position) >= feintDistance)
        {
            // 元の流れに戻る（再出現）
            state = State.Exit;
        }
    }
}

