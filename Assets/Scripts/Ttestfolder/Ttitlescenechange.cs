using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class Ttitlescenechange : MonoBehaviour
{
    public string sceneName;

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
}
