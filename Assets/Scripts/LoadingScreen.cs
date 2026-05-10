using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    // 遷移元から遷移先シーン名をセットする
    public static string nextScene = "";

    private void Start()
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("[LoadingScreen] nextScene が設定されていません。");
            return;
        }

        StartCoroutine(LoadSceneAsync());
    }

    private System.Collections.IEnumerator LoadSceneAsync()
    {
        // 1フレーム待って画面を表示してからロード開始
        yield return null;

        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        while (!op.isDone)
        {
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}