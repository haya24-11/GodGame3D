using UnityEngine;

public class InfiniteFieldScroller : MonoBehaviour
{
    [Header("スクロール方向・速度")]
    public Vector2 scrollDirection = new Vector2(0f, 1f); // X:横 Y:縦
    public float scrollSpeed = 0.5f;

    [Header("テクスチャ拡大率")]
    public Vector2 tiling = new Vector2(1f, 1f); // 数値が大きいほど画像が小さく繰り返す

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
        mat.mainTextureScale = tiling;
    }

    void Update()
    {
        // 方向を正規化して速度をかける
        Vector2 dir = scrollDirection.normalized;
        float offsetX = Mathf.Repeat(Time.time * dir.x * scrollSpeed, 1f);
        float offsetY = Mathf.Repeat(Time.time * dir.y * scrollSpeed, 1f);

        mat.mainTextureOffset = new Vector2(offsetX, offsetY);

        // インスペクターでtilingを変えたとき即反映
        mat.mainTextureScale = tiling;
    }
}