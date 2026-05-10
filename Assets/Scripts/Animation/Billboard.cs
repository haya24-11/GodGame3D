using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null) return;

        // =========================
        // カメラ方向を取得
        // =========================

        Vector3 dir =
            mainCamera.transform.position - transform.position;

        // =========================
        // 上下回転を無視
        // （横方向だけ向く）
        // =========================

        dir.y = 0;

        // =========================
        // カメラ方向を向く
        // =========================

        transform.forward = -dir.normalized;
    }
}