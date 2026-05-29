using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MiniAttack : MonoBehaviour
{
    [SerializeField] private int attack = 2;

    private bool hasHit = false;

    private void OnEnable()
    {
        hasHit = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        IDamageable target =
            other.GetComponentInParent<IDamageable>();

        if (target == null) return;

        hasHit = true;

        target.TakeDamage(
            attack,
            transform.position
        );

        Debug.Log($"[MiniAttack] Hit : {other.name}");
    }
}