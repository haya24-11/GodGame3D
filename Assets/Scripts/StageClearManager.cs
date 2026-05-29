// ============================================
// ファイル：StageClearManager.cs
// 役割：ステージの勝敗管理
// 内容：ボス撃破 / タイム切れ / シーン遷移
// ============================================

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StageClearManager : MonoBehaviour
{
    [Header("監視対象ボス")]
    [SerializeField]
    private BossBase targetBoss;

    [Header("遷移先シーン")]
    [SerializeField]
    private string resultSceneName = "Result";

    [SerializeField]
    private string gameOverSceneName = "GameOver";

    [Header("遷移待機時間")]
    [SerializeField]
    private float resultDelay = 3f;

    [SerializeField]
    private float gameOverDelay = 0f;

    private bool isFinished = false;

    void Start()
    {
        if (targetBoss == null)
        {
            targetBoss = FindObjectOfType<BossBase>();
        }

        if (targetBoss != null)
        {
            targetBoss.OnBossDead += OnBossDead;
        }
        else
        {
            Debug.LogError("[StageClearManager] BossBaseが見つからない");
        }
    }

    void OnDestroy()
    {
        if (targetBoss != null)
        {
            targetBoss.OnBossDead -= OnBossDead;
        }
    }

    void OnBossDead()
    {
        if (isFinished) return;

        isFinished = true;

        StartCoroutine(LoadSceneAfterDelay(
            resultSceneName,
            resultDelay
        ));
    }

    public void OnTimeOver()
    {
        if (isFinished) return;

        isFinished = true;

        StartCoroutine(LoadSceneAfterDelay(
            gameOverSceneName,
            gameOverDelay
        ));
    }

    IEnumerator LoadSceneAfterDelay(
        string sceneName,
        float delay
    )
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(sceneName);
    }
}