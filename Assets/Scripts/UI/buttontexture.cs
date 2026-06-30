using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ButtonTextureSwap : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private Sprite normalSprite;    // 通常時のテクスチャ
    [SerializeField] private Sprite selectedSprite;  // 選択中のテクスチャ

    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
        // 起動時は通常テクスチャにしておく
        if (normalSprite != null) image.sprite = normalSprite;
    }

    // このボタンが選択されたとき
    public void OnSelect(BaseEventData eventData)
    {
        image.sprite = selectedSprite;
    }

    // 選択が外れたとき
    public void OnDeselect(BaseEventData eventData)
    {
        image.sprite = normalSprite;
    }
}