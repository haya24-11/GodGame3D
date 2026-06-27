// どのファイルのどこを変更：BossHPBarUI.cs
// 意図：ボスのHPとスライダーを同期し、表示/非表示も管理できるようにする

using UnityEngine;
using UnityEngine.UI;

public class BossHPBarUI : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    // 意図：どのボスでもHP取得できるようにする
    private System.Func<int> getHp;

    private bool isInitialized = false;

    public void Initialize(System.Func<int> hpGetter, int maxHp)
    {
        getHp = hpGetter;

        if (slider != null)
        {
            slider.maxValue = maxHp;
            slider.value = maxHp;
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;
        if (getHp == null) return;
        if (slider == null) return;

        slider.value = getHp();
    }

    // ============================================
    // 表示
    // ============================================
    public void Show()
    {
        gameObject.SetActive(true);
    }

    // ============================================
    // 非表示
    // ============================================
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}