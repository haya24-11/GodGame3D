// ============================================
// ファイル：EnemyBossStraight.cs
// 継承元：BossBase
//
// 役割：
// ・直進型ボス
// ・画面外出現
// ・停止 → 突進
// ・フェイント
// ・フェーズ変化
// ・HD2D向き制御
//
// 必要コンポーネント：
// ・BossBase
// ・SpriteSheetAnimator
// ・Billboard
// ・Collider
// ============================================

using UnityEngine;
using System.Collections;

public class BossWiper : BossBase
{
    // ============================================
    // 状態
    // ============================================

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

    // ============================================
    // 速度
    // ============================================

    [Header("速度")]

    [SerializeField]
    private float moveSpeed = 2f;

    [SerializeField]
    private float chargeSpeed = 5f;

    // ============================================
    // 移動
    // ============================================

    private Vector3 moveDir;
    private Vector3 startPos;

    private float timer;

    // ============================================
    // フェーズ
    // ============================================

    private bool phase1 = false;
    private bool phase2 = false;

    // ============================================
    // フェイント
    // ============================================

    private bool canFeint = false;

    // フェイント直後は強制直進
    private bool skipNextFeint = false;

    private Vector3 feintStart;

    [SerializeField]
    private float feintDistance = 0.5f;

    // ============================================
    // 吹っ飛び
    // ============================================

    private bool isKnockback = false;

    private Vector3 knockbackDir;

    // ============================================
    // スプライト
    // ============================================

    private SpriteSheetAnimator spriteAnimator;

    // ============================================
    // 出現管理
    // ============================================

    private bool isFirstSpawn = true;

    // ============================================
    // 初期化
    // ============================================

    protected override void Start()
    {
        // ========================================
        // BossBase初期化
        // ========================================

        base.Start();

        isFirstSpawn = true;

        // ========================================
        // SpriteSheetAnimator取得
        // ========================================

        spriteAnimator =
            GetComponentInChildren<SpriteSheetAnimator>();

        // ========================================
        // 初回出現
        // ========================================

        Respawn();
    }

    // ============================================
    // 更新
    // ============================================

    void Update()
    {
        if (isDead) return;

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

    // ============================================
    // スプライト向き更新
    // ============================================

    void UpdateSpriteDirection(Vector3 dir)
    {
        if (spriteAnimator == null) return;

        if (dir != Vector3.zero)
        {
            spriteAnimator.SetDirection(dir);
        }
    }

    // ============================================
    // 出現処理
    // ============================================

    void Respawn()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        // ========================================
        // 初回と再出現で余白変更
        // ========================================

        float margin = isFirstSpawn ? 1f : 1.5f;

        // ========================================
        // 出現方向
        // 0:上
        // 1:下
        // 2:右
        // 3:左
        // ========================================

        int spawnDir = Random.Range(0, 4);

        float x = 0f;
        float z = 0f;

        switch (spawnDir)
        {
            // ====================================
            // 上 → 下
            // ====================================

            case 0:

                x = Random.Range(-w + margin, w - margin);

                z = h + margin;

                moveDir = Vector3.back;

                break;

            // ====================================
            // 下 → 上
            // ====================================

            case 1:

                x = Random.Range(-w + margin, w - margin);

                z = -h - margin;

                moveDir = Vector3.forward;

                break;

            // ====================================
            // 右 → 左
            // ====================================

            case 2:

                x = w + margin;

                z = Random.Range(-h + margin, h - margin);

                moveDir = Vector3.left;

                break;

            // ====================================
            // 左 → 右
            // ====================================

            case 3:

                x = -w - margin;

                z = Random.Range(-h + margin, h - margin);

                moveDir = Vector3.right;

                break;
        }

        // ========================================
        // 位置設定
        // ========================================

        transform.position =
            new Vector3(x, 1f, z);

        isFirstSpawn = false;

        startPos = transform.position;

        // ========================================
        // 向き更新
        // ========================================

        UpdateSpriteDirection(moveDir);

        // ========================================
        // 状態変更
        // ========================================

        state = State.MoveIn;
    }

    // ============================================
    // 侵入
    // ============================================

    void MoveIn()
    {
        transform.Translate(
            moveDir * moveSpeed * Time.deltaTime,
            Space.World
        );

        UpdateSpriteDirection(moveDir);

        // ========================================
        // 0.5unit進んだら停止
        // ========================================

        if (Vector3.Distance(startPos, transform.position) >= 0.5f)
        {
            state = State.Stop;
        }
    }

    // ============================================
    // 停止
    // ============================================

    void Stop()
    {
        timer = 0f;

        state = State.Wait;
    }

    // ============================================
    // 待機
    // ============================================

    void Wait()
    {
        timer += Time.deltaTime;

        if (timer < 1f) return;

        // ========================================
        // フェイント未解禁
        // ========================================

        if (!canFeint)
        {
            Debug.Log("フェイント未解禁");

            state = State.Charge;

            return;
        }

        // ========================================
        // フェイント直後は強制直進
        // ========================================

        if (skipNextFeint)
        {
            Debug.Log("フェイント後の強制直進");

            skipNextFeint = false;

            state = State.Charge;

            return;
        }

        // ========================================
        // 50%でフェイント
        // ========================================

        if (Random.value < 0.5f)
        {
            Debug.Log("フェイント発動");

            feintStart = transform.position;

            skipNextFeint = true;

            state = State.Feint;
        }
        else
        {
            Debug.Log("通常直進");

            state = State.Charge;
        }
    }

    // ============================================
    // 突進
    // ============================================

    void Charge()
    {
        Vector3 dir =
            isKnockback ? knockbackDir : moveDir;

        transform.Translate(
            dir * chargeSpeed * Time.deltaTime,
            Space.World
        );

        UpdateSpriteDirection(dir);

        Camera cam = Camera.main;

        float h = cam.orthographicSize;
        float w = h * cam.aspect;

        // ========================================
        // 画面外へ出たら再出現
        // ========================================

        if (Mathf.Abs(transform.position.z) > h + 2f ||
            Mathf.Abs(transform.position.x) > w + 2f)
        {
            isKnockback = false;

            Respawn();
        }
    }

    // ============================================
    // Exit
    // ============================================

    void Exit()
    {
        Respawn();
    }

    // ============================================
    // フェイント
    // ============================================

    void Feint()
    {
        Vector3 dir = -moveDir;

        transform.Translate(
            dir * moveSpeed * Time.deltaTime,
            Space.World
        );

        UpdateSpriteDirection(dir);

        // ========================================
        // 一定距離戻ったら再出現
        // ========================================

        if (Vector3.Distance(feintStart, transform.position)
            >= feintDistance)
        {
            Respawn();
        }
    }

    // ============================================
    // 被弾時処理
    // BossBaseから呼ばれる
    // ============================================

    protected override void OnDamaged(
        int damage,
        Vector3 attackerPos
    )
    {
        // ========================================
        // 吹っ飛び方向
        // ========================================

        Vector3 dir =
            (transform.position - attackerPos).normalized;

        int t1 =
            Mathf.CeilToInt(maxHp * 2f / 3f);

        int t2 =
            Mathf.CeilToInt(maxHp * 1f / 3f);

        // ========================================
        // Phase1
        // ========================================

        if (!phase1 && currentHp <= t1)
        {
            phase1 = true;

            Debug.Log(
                $"[BossStraight] Phase1突入 HP:{currentHp}"
            );

            moveSpeed = 3f;

            chargeSpeed = 8f;

            canFeint = true;

            isKnockback = true;

            knockbackDir = dir;

            state = State.Charge;

            AddTime(20);
        }

        // ========================================
        // Phase2
        // ========================================

        if (!phase2 && currentHp <= t2)
        {
            phase2 = true;

            Debug.Log(
                $"[BossStraight] Phase2突入 HP:{currentHp}"
            );

            moveSpeed = 4f;

            chargeSpeed = 7f;

            isKnockback = true;

            knockbackDir = dir;

            state = State.Charge;

            AddTime(20);
        }
    }

    // ============================================
    // 死亡処理
    // BossBase override
    // ============================================

    protected override IEnumerator DeathSequence()
    {
        Debug.Log("[BossStraight] 撃破！");

        // ========================================
        // 行動停止
        // ========================================

        state = State.Stop;

        // ========================================
        // 1秒後に非表示
        // ========================================

        yield return new WaitForSeconds(1f);

        if (rend != null)
        {
            rend.enabled = false;
        }

        Collider col = GetComponent<Collider>();

        if (col != null)
        {
            col.enabled = false;
        }

        Debug.Log("[BossStraight] ボス非表示");

        // ========================================
        // 5秒後タイトル
        // ========================================

        yield return new WaitForSeconds(4f);

        Debug.Log("[BossStraight] タイトルへ遷移");

        //UnityEngine.SceneManagement.SceneManager
           // .LoadScene("Title");
    }
}