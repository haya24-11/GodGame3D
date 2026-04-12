using UnityEngine;

public class CursorController : MonoBehaviour
{

    [SerializeField] InputReader inputReader;
    [SerializeField] Mover mover;
    [SerializeField] Camera cam;

    public bool CanMove { get; private set; } = true;

    public void SetMovable(bool movable)
    {
        CanMove = movable;
    }


    void Update()
    {
        if (!CanMove) return;

        Vector2 raw = inputReader.MoveInput;
        if (raw == Vector2.zero) return;

        // カメラのY軸回転だけを取り出す
        float camY = (cam != null ? cam.transform : transform).eulerAngles.y;
        Vector3 camForward = Quaternion.Euler(0, camY, 0) * Vector3.forward;
        Vector3 camRight = Quaternion.Euler(0, camY, 0) * Vector3.right;

        // スティック入力をカメラ基準のワールド方向へ変換
        Vector3 worldDir = (camRight * raw.x + camForward * raw.y).normalized;

        // Moverへはx/zをVector2として渡す
        mover.MoveFromInput(new Vector2(worldDir.x, worldDir.z));
    }
}
