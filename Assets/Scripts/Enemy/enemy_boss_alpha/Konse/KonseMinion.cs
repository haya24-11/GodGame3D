// ============================================
// ファイル：KonseMinion.cs
// 役割：Konse用 minion
// ============================================

using UnityEngine;

public class KonseMinion : MonoBehaviour
{
    private BossKonse owner;

    private GameObject prefab;

    private float speed;

    private Vector3 moveDir;

    private enum MoveType
    {
        Straight,
        Wave
    }

    private MoveType moveType;

    private float waveTimer;

    // ============================================
    // 初期化
    // ============================================

    public void Init(
        BossKonse boss,
        GameObject prefabRef,
        float moveSpeed
    )
    {
        owner = boss;

        prefab = prefabRef;

        speed = moveSpeed;

        waveTimer = 0f;

        moveType =
            Random.value < 0.5f
            ? MoveType.Straight
            : MoveType.Wave;

        moveDir = Vector3.back;
    }

    // ============================================
    // 更新
    // ============================================

    void Update()
    {
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
        transform.Translate(
            moveDir * speed * Time.deltaTime,
            Space.World
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