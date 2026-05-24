// ============================================
// ファイル：LegionClone.cs
// 役割：レギオン分身
// 内容：
// ・X軸往復
// ・Z軸往復
// ・時計回り回転
// ・画面外へ出ない
// ・寿命管理
// ============================================

using UnityEngine;

public class LegionClone : MonoBehaviour
{
    // ============================================
    // 所有者
    // ============================================

    private BossLegion owner;

    // ============================================
    // ステータス
    // ============================================

    private float speed;

    private float lifeTime;

    private float timer;

    // ============================================
    // 移動
    // ============================================

    private Vector3 centerPos;

    [SerializeField]
    private float moveRange = 1f;

    private float orbitAngle;

    // ============================================
    // 行動
    // ============================================

    private enum MoveType
    {
        PingPongX,
        PingPongZ,
        Orbit
    }

    private MoveType moveType;

    // ============================================
    // 初期化
    // ============================================

    public void Init(
        BossLegion boss,
        float spd,
        float life,
        Vector3 startPos
    )
    {
        owner = boss;

        speed = spd;

        lifeTime = life;

        timer = 0f;

        centerPos = startPos;

        orbitAngle =
            Random.Range(0f, 360f);

        // ========================================
        // 行動ランダム
        // ========================================

        int rand =
            Random.Range(0, 3);

        moveType = (MoveType)rand;
    }

    // ============================================
    // 更新
    // ============================================

    void Update()
    {
        if (owner == null)
        {
            return;
        }

        timer += Time.deltaTime;

        // ========================================
        // 寿命
        // ========================================

        if (timer >= lifeTime)
        {
            ReturnToPool();

            return;
        }

        switch (moveType)
        {
            case MoveType.PingPongX:
                PingPongX();
                break;

            case MoveType.PingPongZ:
                PingPongZ();
                break;

            case MoveType.Orbit:
                OrbitMove();
                break;
        }
    }

    // ============================================
    // X往復
    // ============================================

    void PingPongX()
    {
        float x =
            Mathf.Sin(timer * speed)
            * moveRange;

        transform.position =
            centerPos +
            new Vector3(x, 0f, 0f);
    }

    // ============================================
    // Z往復
    // ============================================

    void PingPongZ()
    {
        float z =
            Mathf.Sin(timer * speed)
            * moveRange;

        transform.position =
            centerPos +
            new Vector3(0f, 0f, z);
    }

    // ============================================
    // 時計回り回転
    // ============================================

    void OrbitMove()
    {
        orbitAngle -=
            speed * 60f * Time.deltaTime;

        float rad =
            orbitAngle * Mathf.Deg2Rad;

        transform.position =
            centerPos +
            new Vector3(
                Mathf.Cos(rad),
                0f,
                Mathf.Sin(rad)
            ) * moveRange;
    }

    // ============================================
    // プール返却
    // ============================================

    void ReturnToPool()
    {
        if (owner != null)
        {
            owner.NotifyCloneDead();
        }

        Destroy(gameObject);
    }

    // ============================================
    // 被弾
    // ============================================

    public void TakeDamage()
    {
        ReturnToPool();
    }
}