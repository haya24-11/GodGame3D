// ============================================
// ファイル：LegionClone.cs
// 役割：分身挙動
// ============================================

using UnityEngine;

public class LegionClone : MonoBehaviour
{
    private BossLegion owner;

    private float speed;

    private float lifeTime;

    private float timer;

    private Vector3 moveDir;

    private enum MoveType
    {
        PingPong,
        Orbit
    }

    // ランダムに移動パターンを選択
    private MoveType moveType;

    // 初期化
    public void Init(BossLegion boss, float spd, float life)
    {
        owner = boss;
        speed = spd;
        lifeTime = life;

        timer = 0f;

        moveType =
            (Random.value < 0.5f)
            ? MoveType.PingPong
            : MoveType.Orbit;

        moveDir = Random.insideUnitSphere;
        moveDir.y = 0;
        moveDir.Normalize();
    }

    // 毎フレームの挙動
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            ReturnToPool();
            return;
        }
        
        switch (moveType)
        {
            case MoveType.PingPong:
                PingPongMove();
                break;

            case MoveType.Orbit:
                OrbitMove();
                break;
        }
    }

    // ランダムに方向転換しながら直線移動
    void PingPongMove()
    {
        transform.Translate(
            moveDir * speed * Time.deltaTime,
            Space.World
        );

        if (Random.value < 0.01f)
        {
            moveDir = -moveDir;
        }
    }

    // ボスを中心に回転
    void OrbitMove()
    {
        transform.RotateAround(
            Vector3.zero,
            Vector3.up,
            speed * 20f * Time.deltaTime
        );
    }

    // プールに返却
    void ReturnToPool()
    {
        owner.NotifyCloneDead();

        Destroy(gameObject);
    }

    // 被弾
    public void TakeDamage()
    {
        ReturnToPool();
    }
}