using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private static Stack<string> sceneHistory = new Stack<string>();

    // ↓ Update() は削除。A判定は標準Submitに任せる

    // 各ボタンの onClick にこのメソッドを登録する
    public void ChangeScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("sceneName が未設定 → 直前のシーンへ戻ります");
            GoBackScene();
            return;
        }

        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError("Build Settings にシーンがありません : " + sceneName);
            return;
        }

        sceneHistory.Push(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneName);
    }

    public void GoBackScene()
    {
        if (sceneHistory.Count == 0)
        {
            Debug.LogWarning("戻れるシーンがありません（履歴が空です）");
            return;
        }

        string previousScene = sceneHistory.Pop();
        if (!IsSceneInBuildSettings(previousScene))
        {
            Debug.LogError("Build Settings にシーンがありません : " + previousScene);
            return;
        }
        SceneManager.LoadScene(previousScene);
    }

    private bool IsSceneInBuildSettings(string targetName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == targetName) return true;
        }
        return false;
    }
}