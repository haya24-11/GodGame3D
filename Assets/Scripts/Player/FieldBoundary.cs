using UnityEngine;
using UnityEngine.Rendering;

public class FieldBoundary : MonoBehaviour
{
    [SerializeField] float halfX = 9.6f;
    [SerializeField] float halfZ = 5.4f;

    void LateUpdate()
    {
        Clamp();   
    }

    public void Clamp()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -halfX, halfX);
        pos.z = Mathf.Clamp(pos.z, -halfZ, halfZ);
        transform.position = pos;
    }
}
