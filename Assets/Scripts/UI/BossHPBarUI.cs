// どのファイルのどこを変更：BossHPBarUI.cs（新規）
// 意図：ボスのHPとスライダーを同期する

using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private EnemyBossAlpha boss;

    // ボスから初期化される
    public void Initialize(EnemyBossAlpha targetBoss, int maxHp)
    {
        boss = targetBoss;

        slider.maxValue = maxHp;
        slider.value = maxHp;
    }

    void Update()
    {
        if (boss == null) return;

        slider.value = boss.CurrentHp;
    }
}