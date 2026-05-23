// ============================================
// ファイル：MimesisSlime.cs
// ============================================

using UnityEngine;

public class MimesisSlime : MonoBehaviour
{
    private BossMimesis owner;

    private GameObject prefab;

    private float speed;

    private Vector3 dir;

    public void Init(
        BossMimesis boss,
        GameObject prefabRef,
        float moveSpeed
    )
    {
        owner = boss;

        prefab = prefabRef;

        speed = moveSpeed;

        dir = Random.insideUnitSphere;
        dir.y = 0f;
        dir.Normalize();
    }

    void Update()
    {
        transform.Translate(
            dir * speed * Time.deltaTime,
            Space.World
        );
    }

    public void TakeDamage()
    {
        owner.NotifySlimeDead();

        ObjectPool.Instance.Return(
            prefab,
            gameObject
        );
    }
}