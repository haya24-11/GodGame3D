using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public enum ButtonType
{
    ChangeScene,
    ExitGame
}

public class CreateImageButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    public Canvas canvas;

    public Sprite normalSprite;
    public Sprite selectedSprite;

    [Header("ボタンの種類")]
    public ButtonType buttonType;

    [Header("遷移先シーン名")]
    public string sceneName;

    [Header("位置")]
    public Vector2 position = Vector2.zero;

    [Header("角度")]
    public float angle = 0.0f;

    [Header("サイズ")]
    public Vector2 size = new Vector2(200, 80);

    private Image buttonImage;

    void Start()
    {
        GameObject buttonObj = new GameObject("ImageButton");
        buttonObj.transform.SetParent(canvas.transform, false);

        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        rect.localRotation = Quaternion.Euler(0, 0, angle);

        buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.sprite = normalSprite;

        Button button = buttonObj.AddComponent<Button>();

        // イベント受信用
        buttonObj.AddComponent<PointerEventForwarder>().Initialize(this);
        button.onClick.AddListener(ChangeScene);

        if (buttonType == ButtonType.ExitGame)
        {
            button.onClick.AddListener(ExitGame);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.sprite = selectedSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (buttonImage != null)
            buttonImage.sprite = normalSprite;
    }

    public void ChangeScene()
    {
        Debug.Log("ボタンが押されました");
        SceneManager.LoadScene(sceneName);
    }
    public void ExitGame()
    {
        Debug.Log("ボタンが押されました");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}

public class PointerEventForwarder : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private CreateImageButton owner;

    public void Initialize(CreateImageButton script)
    {
        owner = script;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        owner?.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        owner?.OnPointerExit(eventData);
    }
}