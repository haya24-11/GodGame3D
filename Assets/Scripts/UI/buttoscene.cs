using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string sceneName;

    // シーン遷移の履歴。static なのでシーンをまたいでも保持される
    private static Stack<string> sceneHistory = new Stack<string>();

    void Update()
    {
        // コントローラーが認識されているか確認
        if (Gamepad.current == null)
        {
            Debug.LogWarning("コントローラーが認識されていません");
            return;
        }

        // A ボタン（南）で遷移実行
        // sceneName が入っていればそのシーンへ、空なら直前のシーンへ戻る
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log("Aボタン押された");
            ChangeScene();
        }
    }

    public void ChangeScene()
    {
        Debug.Log("ボタンが押されました");

        // sceneName が空なら「直前のシーンに戻る」動作にする
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.Log("sceneName が未設定 → 直前のシーンへ戻ります");
            GoBackScene();
            return;
        }

        Debug.Log("遷移先シーン : " + sceneName);

        // BuildSettings に存在するか確認
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogError("Build Settings にシーンがありません : " + sceneName);
            return;
        }

        // 遷移する前に「今いるシーン」を履歴に積む
        sceneHistory.Push(SceneManager.GetActiveScene().name);

        Debug.Log("シーン遷移開始");
        SceneManager.LoadScene(sceneName);
    }

    // 直前のシーンに戻る
    public void GoBackScene()
    {
        // 履歴が空なら戻れない
        if (sceneHistory.Count == 0)
        {
            Debug.LogWarning("戻れるシーンがありません（履歴が空です）");
            return;
        }

        string previousScene = sceneHistory.Pop();
        Debug.Log("直前のシーンに戻る : " + previousScene);

        // 念のため BuildSettings 存在チェック
        if (!IsSceneInBuildSettings(previousScene))
        {
            Debug.LogError("Build Settings にシーンがありません : " + previousScene);
            return;
        }

        SceneManager.LoadScene(previousScene);
    }

    // BuildSettings にシーンが含まれているか確認する共通処理
    private bool IsSceneInBuildSettings(string targetName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == targetName)
            {
                return true;
            }
        }
        return false;
    }
}