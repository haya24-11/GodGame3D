using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName; // インスペクターでシーンの名前を入力

    void Update()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightTrigger.wasPressedThisFrame)
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
    //ここから下は旧式のやつ
 /*
    public void LoadScene()
    {
        Debug.Log("シーン遷移可");
        SceneManager.LoadScene(sceneName);
        LoadingScreen.nextScene = "Samplescene";
    }
 */
}