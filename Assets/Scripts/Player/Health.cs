using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int maxHP = 100;

    public int CurrentHP { get; private set; }

    public event Action OnDead;

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        CurrentHP = Mathf.Max(0, CurrentHP - amount);

        if(CurrentHP==0)
        {
            OnDead?.Invoke();
        }
    }
}
