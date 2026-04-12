using UnityEngine;
using UnityEngine.InputSystem;
public class Tcamerascript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("ターゲット")]
    [SerializeField] private Transform target;//UNITYで変更可

    [Header("回転速度")]
    [SerializeField] private float rotationSpeed = 100f;//UNITYで変更可

    private float currentAngle = 0f;
    private Vector3 offset; // 初期位置からの差

    void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
            currentAngle = Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        float input = 0f;
        //キーボード
        if (Gamepad.current != null)
        {
            input = Gamepad.current.rightStick.ReadValue().x;
        }
        //コントローラー
        if (Gamepad.current != null)
        {
            input = Gamepad.current.rightStick.ReadValue().x;
        }

        currentAngle += input * rotationSpeed * Time.deltaTime;

        Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
        Vector3 newOffset = rotation * offset;

        transform.position = target.position + newOffset;
        transform.LookAt(target);
    }
}
