// ============================================
// ファイル：KonseMinion.cs
// 役割：Konse用 minion
// ============================================

using UnityEngine;

public class KonseMinion : MonoBehaviour, IDamageable
{
    private BossKonse owner;    // 親Boss

    private GameObject prefab;  // 自身のPrefab参照

    private float speed;    // 移動速度

    private Vector3 moveDir;    // 移動方向
    private Vector3 waveDir;    // 波移動の揺れ方向

    [SerializeField]
    private float waveAmplitude = 1f;   //  波移動の振幅

    [SerializeField]
    private float waveFrequency = 5f;   //  波移動の周波数

    public enum MoveType
    {   // 移動タイプ
        Straight,
        Wave
    }

    private MoveType moveType;  // 移動タイプ

    private float waveTimer;    // 波移動用タイマー 波の位相として使用

    private bool isReturning = false;   // Pool返却中フラグ

    // ============================================
    // 初期化
    // BossKonseから呼ばれる
    // ============================================

    public void Init(
        BossKonse boss, // 親Boss
        GameObject prefabRef,   //  自身のPrefab参照
        float moveSpeed, //  移動速度
        Vector3 dir,
         Vector3 waveDirection,
          MoveType selectedMoveType
    )
    {
        owner = boss;   //  親Bossをセット
        prefab = prefabRef; //  自身のPrefab参照をセット
        speed = moveSpeed;  //  移動速度をセット
        waveTimer = 0f; //  波移動用タイマーをリセット

        moveDir = dir.normalized; //  移動方向をセット（Z軸負方向）
        waveDir = waveDirection.normalized;

        //  移動タイプをランダムに決定
        moveType = selectedMoveType;

        isReturning = false;    //  Pool返却中フラグをリセット
    }

    // ============================================
    // 更新
    // ============================================

    void Update()
    {
        // 移動タイプに応じた移動処理を実行
        switch (moveType)
        {
            case MoveType.Straight:
                StraightMove();
                break;

            case MoveType.Wave:
                WaveMove();
                break;
        }

        CheckOutScreen();
    }

    // ============================================
    // 直進
    // ============================================

    void StraightMove()
    {
        transform.Translate(    //  移動
            moveDir * speed * Time.deltaTime,   //  移動量
            Space.World //  ワールド座標で移動
        );
    }

    // ============================================
    // 波移動
    // ============================================

    void WaveMove()
    {
        waveTimer += Time.deltaTime;

        Vector3 pos = transform.position;

        // 前進
        pos += moveDir * speed * Time.deltaTime;

        // 出現方向に応じた横揺れ
        float waveVelocity =
            Mathf.Cos(waveTimer * waveFrequency)
            * waveAmplitude
            * waveFrequency;

        pos += waveDir * waveVelocity * Time.deltaTime;

        transform.position = pos;
    }

    // ============================================
    // 画面外判定
    // ============================================

    void CheckOutScreen()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize + 2f;
        float w = h * cam.aspect;

        Vector3 pos = transform.position;

        if (Mathf.Abs(pos.x) > w ||
            Mathf.Abs(pos.z) > h)
        {
            ReturnToPool();
        }
    }

    // ============================================
    // 被弾
    // ============================================
    public void TakeDamage(
    int damage,
    Vector3 attackerPos
)
    {
        Debug.Log("[KonseMinion] 被弾");
        ReturnByDamage();
    }
    void ReturnByDamage()
    {
        if (isReturning) return;

        isReturning = true;

        if (owner != null)
        {
            owner.NotifyMinionDead(this);
        }

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return(prefab, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // ============================================
    // Pool返却
    // ============================================
    void ReturnToPool()
    {
        if (isReturning) return;

        isReturning = true;

        if (owner != null)
        {
            owner.RequestRespawnFormation(1f);
            return;
        }

        if (ObjectPool.Instance != null)
        {
            ObjectPool.Instance.Return(prefab, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
    