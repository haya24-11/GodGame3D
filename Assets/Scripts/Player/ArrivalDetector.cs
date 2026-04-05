using UnityEngine;
using System;

public class ArrivalDetector : MonoBehaviour
{
    [SerializeField] float arrivalThreshold = 0.1f;

    public event Action OnArrived;

    Vector3 targetPosition;
    bool isTracking = false;

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        isTracking = true;
    }

    void Update()
    {
        if (!isTracking) return;

        if(Vector3.Distance(transform.position, targetPosition) < arrivalThreshold)
        {
            isTracking = false;

            OnArrived?.Invoke();
        }
    }
}
