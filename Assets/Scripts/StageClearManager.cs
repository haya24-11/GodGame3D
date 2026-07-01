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
        // Prefab生成型の場合、ここでは無理にFindしない
        // スポナー側からSetTargetBoss()で登録する
        if (targetBoss != null)
        {
            SetTargetBoss(targetBoss);
        }
        else
        {
            Debug.Log("[StageClearManager] ボス登録待機中");
        }
    }

    void OnDestroy()
    {
        if (targetBoss != null)
        {
            targetBoss.OnBossDead -= OnBossDead;
        }
    }

    // ============================================
    // 生成されたボスを登録
    // 意図：Prefabではなく、実際に生成されたBoss(Clone)を監視する
    // ============================================
    public void SetTargetBoss(BossBase boss)
    {
        if (boss == null)
        {
            Debug.LogError("[StageClearManager] 登録するBossがnull");
            return;
        }

        if (targetBoss != null)
        {
            targetBoss.OnBossDead -= OnBossDead;
        }

        targetBoss = boss;

        targetBoss.OnBossDead += OnBossDead;

        Debug.Log(
            $"[StageClearManager] 監視対象登録:{targetBoss.gameObject.name}"
        );
    }

    void OnBossDead()
    {
        if (isFinished) return;

        Debug.Log("[StageClearManager] ボス撃破通知を受信");

        isFinished = true;

        StartCoroutine(LoadSceneAfterDelay(
            resultSceneName,
            resultDelay
        ));
    }

    public void OnTimeOver()
    {
        if (isFinished) return;

        Debug.Log("[StageClearManager] タイムオーバー");

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

        Debug.Log($"[StageClearManager] シーン遷移:{sceneName}");

        SceneManager.LoadScene(sceneName);
    }
}