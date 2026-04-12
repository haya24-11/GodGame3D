using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string sceneName; // インスペクターでシーンの名前を入力

    public void LoadScene()
    {
        Debug.Log("シーン遷移可");
        SceneManager.LoadScene(sceneName);
    }
}