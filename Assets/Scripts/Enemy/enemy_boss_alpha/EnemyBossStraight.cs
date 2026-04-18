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

    // 意図：吹っ飛び方向制御
    private Vector3 knockbackDir;
    private bool isKnockback = false;

    protected override void Start()
    {
        base.Start();
        state = State.Spawn;
        Respawn();
    }

    private State prevState;
    void Update()
    {
        // 意図：状態確認
        if (prevState != state)
        {
            Debug.Log($"State変更: {prevState} → {state}");
            prevState = state;
        }
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
                FeintStart();
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
        // 意図：通常移動と吹っ飛びを分岐
        Vector3 dir = isKnockback ? knockbackDir : moveDir;
        transform.Translate(dir * chargeSpeed * Time.deltaTime);

        // 画面外判定
        var cam = Camera.main;
        float h = cam.orthographicSize;

        if (Mathf.Abs(transform.position.z) > h + 2f ||
    Mathf.Abs(transform.position.x) > (h * Camera.main.aspect) + 2f)
        {
            isKnockback = false;
            state = State.Exit;
        }
    }

    void Exit()
    {
        Respawn();
    }

    // 意図：吹っ飛び＋時間加算＋フェイント解禁
    void Phase1(Vector3 dir)
    {
        Debug.Log($"[BossStraight] Phase1突入 HP:{currentHp}");

        moveSpeed = 3f;
        canFeint = true;

        // 吹っ飛び
        knockbackDir = dir;
        chargeSpeed = 8f;
        isKnockback = true;

        // 時間+20
        SendMessage("AddTime", 20, SendMessageOptions.DontRequireReceiver);

        state = State.Charge;
    }

    // 意図：さらに高速化＋吹っ飛び
    void Phase2(Vector3 dir)
    {
        Debug.Log($"[BossStraight] Phase2突入 HP:{currentHp}");

        moveSpeed = 4f;
        chargeSpeed = 7f;

        knockbackDir = dir;
        isKnockback = true;

        // 時間+20
        SendMessage("AddTime", 20, SendMessageOptions.DontRequireReceiver);

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
            skipNextFeint = false;
        }
    }

    // 意図：攻撃方向を受け取る
    public void TakeDamage(int damage, Vector3 attackerPos)
    {
        int beforeHp = currentHp;

        Debug.Log($"[BossStraight] ダメージ受け取り前 HP:{beforeHp}");

        currentHp -= damage;

        int afterHp = currentHp;

        Debug.Log($"[BossStraight] ダメージ受け取り後 HP:{afterHp}");
        // 吹っ飛び方向
        Vector3 dir = (transform.position - attackerPos).normalized;

        // フェーズ判定
        int threshold1 = Mathf.CeilToInt(maxHp * 2f / 3f);
        int threshold2 = Mathf.CeilToInt(maxHp * 1f / 3f);

        Debug.Log($"[BossStraight] 閾値確認 before:{beforeHp} after:{afterHp} t1:{threshold1} t2:{threshold2}");

        if (beforeHp > threshold1 && afterHp <= threshold1)
        {
            Debug.Log("[BossStraight] Phase1条件成立");
            Phase1(dir);
        }

        if (beforeHp > threshold2 && afterHp <= threshold2)
        {
            Debug.Log("[BossStraight] Phase2条件成立");
            Phase2(dir);
        }

        if (currentHp <= 0)
        {
            OnDead();
        }
    }
}

