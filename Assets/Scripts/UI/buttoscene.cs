using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField] private string sceneName;

    void Update()
    {
        // コントローラーが認識されているか確認
        if (Gamepad.current == null)
        {
            Debug.LogWarning("コントローラーが認識されていません");
            return;
        }
        else
        {
            Debug.Log("コントローラー認識OK: " + Gamepad.current.name);
        }

        // ボタンを押したら確認
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            Debug.Log("Aボタン押された");
        }
        if (Gamepad.current.dpad.right.wasPressedThisFrame)
        {
            Debug.Log("十字キー右 押された");
        }
    }

    public void ChangeScene()
    {
        Debug.Log("ボタンが押されました");

        // シーン名が空か確認
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("sceneName が設定されていません！");
            return;
        }

        Debug.Log("遷移先シーン : " + sceneName);

        // BuildSettingsに存在するか確認
        bool sceneExists = false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);

            if (name == sceneName)
            {
                sceneExists = true;
                break;
            }
        }

        if (!sceneExists)
        {
            Debug.LogError("Build Settings にシーンがありません : " + sceneName);
            return;
        }

        Debug.Log("シーン遷移開始");

        SceneManager.LoadScene(sceneName);
    }


}