// どのファイルのどこを変更：EnemyBossStraight.cs（新規）
// 意図：状態管理ベースを構築
// 意図：ボスの全挙動・フェーズ・ダメージ・点滅を一本で管理

using UnityEngine;

public class EnemyBossStraight : MonoBehaviour
{
    private enum State
    {
        MoveIn,
        Stop,
        Wait,
        Charge,
        Exit,
        Feint
    }

    private State state;

    [Header("ステータス")]
    [SerializeField] private int maxHp = 60;
    private int currentHp;

    [Header("速度")]
    [SerializeField] private float moveSpeed = 2f;
    private float baseMoveSpeed = 2f;
    private float chargeSpeed = 5f;

    private Vector3 moveDir;
    private Vector3 startPos;

    private float timer;

    // フェーズ
    private bool phase1 = false;
    private bool phase2 = false;

    // フェイント
    private bool canFeint = false;
    private bool skipNextFeint = false;
    private Vector3 feintStart;
    private float feintDistance = 0.5f;

    // 吹っ飛び
    private bool isKnockback = false;
    private Vector3 knockbackDir;

    // 点滅
    private Renderer rend;
    private Color originalColor;

    // 意図：初回か再出現かを判定
    private bool isFirstSpawn = true;

    public int CurrentHp => currentHp;
    void Start()
    {
        currentHp = maxHp;
        isFirstSpawn = true;

        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;

        // HPバー接続
        var ui = FindObjectOfType<BossHPBarUI>();
        if (ui != null)
        {
            ui.Initialize(() => currentHp, maxHp);
        }

        Respawn();
    }

    void Update()
    {
        switch (state)
        {
            case State.MoveIn: MoveIn(); break;
            case State.Stop: Stop(); break;
            case State.Wait: Wait(); break;
            case State.Charge: Charge(); break;
            case State.Exit: Exit(); break;
            case State.Feint: Feint(); break;
        }
    }

    // =========================
    // 出現処理
    // =========================
    void Respawn()
    {
        var cam = Camera.main;
        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        // 意図：初回は1unit、再出現は1.5unit
        float margin = isFirstSpawn ? 1f : 1.5f;

        // どの方向から出るか（0:上 1:下 2:右 3:左）
        int spawnDir = Random.Range(0, 4);

        float x = 0f;
        float z = 0f;

        switch (spawnDir)
        {
            case 0: // 上 → 下
                x = Random.Range(-w + margin, w - margin);
                z = h + margin;
                moveDir = Vector3.back;
                break;

            case 1: // 下 → 上
                x = Random.Range(-w + margin, w - margin);
                z = -h - margin;
                moveDir = Vector3.forward;
                break;

            case 2: // 右 → 左
                x = w + margin;
                z = Random.Range(-h + margin, h - margin);
                moveDir = Vector3.left;
                break;

            case 3: // 左 → 右
                x = -w - margin;
                z = Random.Range(-h + margin, h - margin);
                moveDir = Vector3.right;
                break;
        }
        transform.position = new Vector3(x, 1f, z);
        isFirstSpawn = false;

        startPos = transform.position;

        state = State.MoveIn;
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

        if (timer >= 1f)
        {
            // フェイント
            if (canFeint)
            {
                // フェイント後は強制で直進
                if (skipNextFeint)
                {
                    Debug.Log("フェイント後の強制直進");
                    state = State.Charge;
                    skipNextFeint = false;
                    return;
                }

                // 50%でフェイント
                if (Random.value < 0.5f)
                {
                    Debug.Log("フェイント発動");
                    feintStart = transform.position;
                    state = State.Feint;

                    // 次は必ず直進
                    skipNextFeint = true;
                }
                else
                {
                    Debug.Log("通常直進");
                    state = State.Charge;
                }
            }
            else
            {
                Debug.Log("フェイント未解禁（Phase前）");
                state = State.Charge;
            }
        }
    }

    void Charge()
    {
        Vector3 dir = isKnockback ? knockbackDir : moveDir;

        transform.Translate(dir * chargeSpeed * Time.deltaTime);

        var cam = Camera.main;
        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        if (Mathf.Abs(transform.position.z) > h + 2f ||
            Mathf.Abs(transform.position.x) > w + 2f)
        {
            isKnockback = false;
            Respawn();
        }
    }

    void Exit()
    {
        Respawn();
    }

    void Feint()
    {
        transform.Translate(-moveDir * moveSpeed * Time.deltaTime);

        if (Vector3.Distance(feintStart, transform.position) >= feintDistance)
        {
            Respawn();
            // ここは触らない
            // skipNextFeint は Waitで管理する
        }
    }

    // =========================
    // ダメージ処理
    // =========================
    public void TakeDamage(int damage, Vector3 attackerPos)
    {

        int beforeHp = currentHp;
        currentHp -= damage;
        Debug.Log($"[BossStraight] ダメージ:{damage} / HP:{beforeHp} → {currentHp}");

        // 点滅
        StartCoroutine(DamageFlash());

        // 吹っ飛び方向
        Vector3 dir = (transform.position - attackerPos).normalized;

        int t1 = Mathf.CeilToInt(maxHp * 2f / 3f);
        int t2 = Mathf.CeilToInt(maxHp * 1f / 3f);

        // Phase1
        if (!phase1 && currentHp <= t1)
        {
            phase1 = true;
            Debug.Log($"[BossStraight] Phase1突入 HP:{currentHp}");

            moveSpeed = 3f;
            chargeSpeed = 8f;
            canFeint = true;

            isKnockback = true;
            knockbackDir = dir;

            //  即吹っ飛び状態へ
            state = State.Charge;

            SendMessage("AddTime", 20, SendMessageOptions.DontRequireReceiver);
        }

        // Phase2
        if (!phase2 && currentHp <= t2)
        {
            phase2 = true;
            Debug.Log($"[BossStraight] Phase2突入 HP:{currentHp}");

            moveSpeed = 6f;
            chargeSpeed = 7f;

            isKnockback = true;
            knockbackDir = dir;

            //  即吹っ飛び状態へ
            state = State.Charge;

            SendMessage("AddTime", 20, SendMessageOptions.DontRequireReceiver);
        }

        if (currentHp <= 0)
        {
            Destroy(gameObject);
        }
    }

    // =========================
    // 点滅処理
    // =========================
    System.Collections.IEnumerator DamageFlash()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
    }
}
