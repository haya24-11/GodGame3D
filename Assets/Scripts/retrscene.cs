using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UIボタンに付けるだけで「前のシーンに戻る」ボタンになるコンポーネント。
/// </summary>
[RequireComponent(typeof(Button))]
public class BackButton : MonoBehaviour
{
    [Header("前のシーンがない場合に非表示にするか")]
    [SerializeField] private bool hideIfNoPreviousScene = true;

    private Button button;

    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClickBack);
    }

    void Start()
    {
        UpdateButtonVisibility();
    }

    private void OnClickBack()
    {
        if (SceneHistory.Instance == null)
        {
            Debug.LogError("SceneHistory が見つかりません。シーンに SceneHistoryInitializer を置いてください。");
            return;
        }
        SceneHistory.Instance.GoBack();
    }

    private void UpdateButtonVisibility()
    {
        if (!hideIfNoPreviousScene) return;

        if (SceneHistory.Instance != null)
        {
            gameObject.SetActive(SceneHistory.Instance.HasPreviousScene());
        }
    }
}