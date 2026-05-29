using UnityEngine;

/// <summary>
/// 星用の球体をカメラに追従させつつ、ゆっくり回転させる。
/// 空そのものはSkybox、星はこのStarSphereで別管理する。
/// </summary>
public class StarSphereRotator : MonoBehaviour
{
    [Header("追従対象。基本はMain Camera")]
    [SerializeField] private Transform targetCamera;

    [Header("星の回転速度")]
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 1f, 0f);

    private void Start()
    {
        if (targetCamera == null && Camera.main != null)
        {
            targetCamera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        // 星の球体を常にカメラ位置へ移動させる。
        // これにより、プレイヤーが移動しても星が近づいたり遠ざかったりしない。
        if (targetCamera != null)
        {
            transform.position = targetCamera.position;
        }

        // 星だけをゆっくり回転させる。
        transform.Rotate(rotationSpeed * Time.deltaTime, Space.Self);
    }
}