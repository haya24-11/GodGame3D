using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;

    Vector3 targetPosition;
    bool isTracking = false;

    public void MoveFromInput(Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y);
        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;
    }

    void Update()
    {
        if (!isTracking) return;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
}
