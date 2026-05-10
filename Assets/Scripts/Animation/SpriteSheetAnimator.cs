using UnityEngine;

public class SpriteSheetAnimator : MonoBehaviour
{
    [Header("SpriteSheet設定")]
    public int columns = 3;
    public int rows = 4;

    [Header("アニメ速度")]
    public float fps = 8f;

    [Header("現在の向き")]
    /*
        0 = 下
        1 = 左
        2 = 右
        3 = 上
    */
    [Range(0, 3)]
    public int directionRow = 0;

    private Renderer rend;

    private int currentFrame;
    private float timer;

    void Start()
    {
        rend = GetComponent<Renderer>();

        Vector2 size = new Vector2(
            1f / columns,
            1f / rows
        );

        rend.material.mainTextureScale = size;

        UpdateUV();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / fps)
        {
            timer = 0f;

            currentFrame++;

            if (currentFrame >= columns)
            {
                currentFrame = 0;
            }

            UpdateUV();
        }
    }

    void UpdateUV()
    {
        int x = currentFrame;
        int y = directionRow;

        Vector2 offset = new Vector2(
            (float)x / columns,
            1f - ((float)y + 1) / rows
        );

        rend.material.mainTextureOffset = offset;
    }

    // 外部から向きを変更する用
    public void SetDirection(Vector3 moveDir)
    {
        // 上下左右どっちが強いか判定
        if (Mathf.Abs(moveDir.x) > Mathf.Abs(moveDir.z))
        {
            // 左右向き
            if (moveDir.x > 0)
            {
                directionRow = 1; // 右
            }
            else
            {
                directionRow = 2; // 左
            }
        }
        else
        {
            // 上下向き
            if (moveDir.z > 0)
            {
                directionRow = 0; // 上
            }
            else
            {
                directionRow = 3; // 下
            }
        }
    }
}