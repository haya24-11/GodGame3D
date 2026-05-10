using UnityEngine;

public class ShadowController : MonoBehaviour
{
    [Header("影サイズ")]
    [SerializeField]
    private float shadowScale = 1.5f;

    [Header("地面からの高さ")]
    [SerializeField]
    private float groundOffset = 0.01f;

    private void Start()
    {
        UpdateShadow();
    }

    void UpdateShadow()
    {
        // =========================
        // サイズ調整
        // =========================

        transform.localScale = new Vector3(
            shadowScale,
            shadowScale,
            shadowScale
        );

        // =========================
        // 地面へ配置
        // =========================

        Vector3 pos = transform.localPosition;

        pos.y = groundOffset;

        transform.localPosition = pos;
    }

    // =========================
    // 外部からサイズ変更
    // =========================

    public void SetShadowSize(float size)
    {
        shadowScale = size;

        UpdateShadow();
    }
}