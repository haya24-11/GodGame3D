// 意図：全敵の共通処理をまとめる

using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] protected int maxHp = 10;

    [SerializeField] protected Ttimer addtimer;
    public int addTimeOnDeath = 10;


    protected int currentHp;

    protected virtual void Start()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(int damage)
    {
        currentHp -= damage;

        if (currentHp <= 0)
        {
            OnDead();
        }
    }

    protected virtual void OnDead()
    {

        if (addtimer != null)
        {
            addtimer.AddTime(addTimeOnDeath);
        }

        Destroy(gameObject);
    }

    public float HpRate => (float)currentHp / maxHp;
}