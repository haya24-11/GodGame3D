// 意図：enemy_boss_alpha から参照されるための最低限の攻撃力データを持つ

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MiniAttack : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private int attack = 2; // 仕様書：攻撃力2

    // 外部（ボスなど）から参照する用
    public int Attack => attack;
}