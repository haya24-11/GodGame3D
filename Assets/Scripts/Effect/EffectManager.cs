using UnityEngine;

public class EffectManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static EffectManager Instance;

    void Awake()
    {
        Instance = this;
    }

    // 共通関数
    public void PlayEffect(GameObject effectPrefab, Vector3 position, float destroyTime = 2f)
    {
        GameObject fx = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(fx, destroyTime);
    }
}
