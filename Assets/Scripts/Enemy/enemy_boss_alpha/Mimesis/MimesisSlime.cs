// ============================================
// ファイル：MimesisSlime.cs
// 役割：Mimesis用Slime
// 内容：KonseMinionと同じ隊列移動
// ============================================

using UnityEngine;

public class MimesisSlime : MonoBehaviour, IDamageable
{
    private BossMimesis owner;
    private GameObject prefab;

    private float speed;

    private Vector3 moveDir;
    private Vector3 waveDir;

    [SerializeField]
    private float waveAmplitude = 1f;

    [SerializeField]
    private float waveFrequency = 5f;

    public enum MoveType
    {
        Straight,
        Wave
    }

    private MoveType moveType;

    private float waveTimer;
    private bool isReturned = false;

    public void Init(
        BossMimesis boss,
        GameObject prefabRef,
        float moveSpeed,
        Vector3 dir,
        Vector3 waveDirection,
        MoveType selectedMoveType
    )
    {
        owner = boss;
        prefab = prefabRef;
        speed = moveSpeed;

        moveDir = dir.normalized;
        waveDir = waveDirection.normalized;

        moveType = selectedMoveType;

        waveTimer = 0f;
        isReturned = false;
    }

    void Update()
    {
        if (isReturned) return;

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

    void StraightMove()
    {
        transform.Translate(
            moveDir * speed * Time.deltaTime,
            Space.World
        );
    }

    void WaveMove()
    {
        waveTimer += Time.deltaTime;

        Vector3 pos = transform.position;

        pos += moveDir * speed * Time.deltaTime;

        float waveVelocity =
            Mathf.Cos(waveTimer * waveFrequency)
            * waveAmplitude
            * waveFrequency;

        pos += waveDir * waveVelocity * Time.deltaTime;

        transform.position = pos;
    }

    void CheckOutScreen()
    {
        Camera cam = Camera.main;

        float h = cam.orthographicSize + 2f;
        float w = h * cam.aspect;

        Vector3 pos = transform.position;

        if (Mathf.Abs(pos.x) > w ||
            Mathf.Abs(pos.z) > h)
        {
            ReturnToPoolByOutScreen();
        }
    }

    public void TakeDamage(
     int damage,
     Vector3 attackerPos
 )
    {
        if (isReturned) return;

        isReturned = true;

        if (owner != null)
        {
            owner.NotifySlimeDead();
        }

        ReturnToPool();
    }
    void ReturnToPoolByOutScreen()
    {
        if (isReturned) return;

        isReturned = true;

        if (owner != null)
        {
            owner.NotifySlimeOutScreen();
        }

        ReturnToPool();
    }

    void ReturnToPool()
    {
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