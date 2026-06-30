using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン遷移の履歴を管理するシングルトン。
/// DontDestroyOnLoadで全シーンをまたいで動作します。
/// </summary>
public class SceneHistory : MonoBehaviour
{
    public static SceneHistory Instance { get; private set; }

    // 前のシーン名を保存
    private string previousSceneName = "";
    private string currentSceneName = "";

    void Awake()
    {
        // シングルトン設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentSceneName = SceneManager.GetActiveScene().name;

        // シーン変更イベントを登録
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene oldScene, Scene newScene)
    {
        previousSceneName = oldScene.name;
        currentSceneName = newScene.name;
    }

    /// <summary>前のシーンに戻ります</summary>
    public void GoBack()
    {
        if (string.IsNullOrEmpty(previousSceneName))
        {
            Debug.LogWarning("前のシーンが記録されていません。");
            return;
        }
        SceneManager.LoadScene(previousSceneName);
    }

    /// <summary>前のシーン名を取得します</summary>
    public string GetPreviousSceneName() => previousSceneName;

    /// <summary>前のシーンがあるか確認します</summary>
    public bool HasPreviousScene() => !string.IsNullOrEmpty(previousSceneName);
}