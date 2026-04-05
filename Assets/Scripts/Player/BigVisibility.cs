using UnityEngine;

public class BigVisibility : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
