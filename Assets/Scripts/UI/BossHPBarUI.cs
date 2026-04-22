// どのファイルのどこを変更：BossHPBarUI.cs（新規）
// 意図：ボスのHPとスライダーを同期する

using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;

    private EnemyBossAlpha boss;

    // ボスから初期化される
    // 意図：どのボスでもHP取得できるようにする
    private System.Func<int> getHp;

    public void Initialize(System.Func<int> hpGetter, int maxHp)
    {
        slider.maxValue = maxHp;
        getHp = hpGetter;
    }

    void Update()
    {
        if (getHp != null)
        {
            slider.value = getHp();
        }
    }
}