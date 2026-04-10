using UnityEngine;

public class enemy_hp : MonoBehaviour
{
    [SerializeField] private int maxHP = 10;

    private int currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        Debug.Log($"{gameObject.name} が {damage} ダメージ受けた。残りHP: {currentHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} が倒された");

        // ここで破壊・非アクティブ化・プール返却など
        Destroy(gameObject);
    }
}
