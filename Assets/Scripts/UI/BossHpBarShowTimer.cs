// ============================================
// ファイル：BossHPBarShowTimer.cs
// 役割：ボスHPバー表示タイマー
// 内容：
// ・ステージ開始時はHPバー非表示
// ・指定時間経過後にHPバー表示
// ・ボス出現処理とは分離
// ============================================

using UnityEngine;

public class BossHPBarShowTimer : MonoBehaviour
{
    [Header("HPバーUI")]

    [SerializeField]
    private BossHPBarUI bossHPBarUI;

    [Header("表示時間")]

    [SerializeField]
    private float showTime = 30f;

    private float timer = 0f;

    private bool hasShown = false;

    void Start()
    {
        if (bossHPBarUI != null)
        {
            bossHPBarUI.Hide();
        }
    }

    void Update()
    {
        if (hasShown) return;

        timer += Time.deltaTime;

        if (timer >= showTime)
        {
            ShowBossHPBar();
        }
    }

    void ShowBossHPBar()
    {
        hasShown = true;

        if (bossHPBarUI != null)
        {
            bossHPBarUI.Show();
        }

        Debug.Log("[BossHPBarShowTimer] ボスHPバー表示");
    }
}