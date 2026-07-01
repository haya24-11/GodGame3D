using UnityEngine;
using UnityEngine.UI;

public class UISpriteSheetAnimation : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] frames;
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private bool loop = true;
    [SerializeField] private bool useUnscaledTime = true;

    private int currentFrame;
    private float timer;

    private void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Update()
    {
        if (targetImage == null || frames == null || frames.Length == 0)
            return;

        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

        timer += deltaTime;

        float frameDuration = 1f / frameRate;

        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Length - 1;
                    enabled = false;
                }
            }

            targetImage.sprite = frames[currentFrame];
        }
    }
}