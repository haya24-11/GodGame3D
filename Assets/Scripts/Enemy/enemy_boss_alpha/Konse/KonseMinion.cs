// ============================================
// ファイル：KonseMinion.cs
// 役割：Konse用 minion
// ============================================

using UnityEngine;

public class KonseMinion : MonoBehaviour
{
    private BossKonse owner;    // 親Boss

    private GameObject prefab;  // 自身のPrefab参照

    private float speed;    // 移動速度

    private Vector3 moveDir;    // 移動方向

    private enum MoveType
    {   // 移動タイプ
        Straight,
        Wave
    }

    private MoveType moveType;  // 移動タイプ

    private float waveTimer;    // 波移動用タイマー 波の位相として使用

    // ============================================
    // 初期化
    // BossKonseから呼ばれる
    // ============================================

    public void Init(
        BossKonse boss, // 親Boss
        GameObject prefabRef,   //  自身のPrefab参照
        float moveSpeed, //  移動速度
        Vector3 dir
    )
    {
        owner = boss;   //  親Bossをセット

        prefab = prefabRef; //  自身のPrefab参照をセット

        speed = moveSpeed;  //  移動速度をセット

        waveTimer = 0f; //  波移動用タイマーをリセット

        moveDir = dir.normalized; //  移動方向をセット（Z軸負方向）

        //  移動タイプをランダムに決定
        moveType =
            Random.value < 0.5f
            ? MoveType.Straight
            : MoveType.Wave;
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

        pos += moveDir * speed * Time.deltaTime;

        pos.x += Mathf.Sin(waveTimer * 5f)
            * Time.deltaTime;

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

    public void TakeDamage()
    {
        ReturnToPool();
    }

    // ============================================
    // Pool返却
    // ============================================

    void ReturnToPool()
    {
        if (owner != null)
        {
            owner.NotifyMinionDead();
        }

        ObjectPool.Instance.Return(prefab, gameObject);
    }
}