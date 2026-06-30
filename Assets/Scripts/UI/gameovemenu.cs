using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected; // リトライボタンを割り当て

    private GameObject lastLogged; // 前フレームの選択を覚えておく（ログ連発防止）

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    void Update()
    {
        var current = EventSystem.current.currentSelectedGameObject;

        // 選択が変わった時だけログを出す
        if (current != lastLogged)
        {
            if (current != null)
                Debug.Log("選択中のボタン : " + current.name);
            else
                Debug.Log("選択なし（null）");

            lastLogged = current;
        }
    }
}