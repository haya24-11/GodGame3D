using UnityEngine;
using System;
using System.Collections.Generic;

public class MiniSpawner : MonoBehaviour
{
    [SerializeField] GameObject miniPrefab;
    [SerializeField] int miniCount = 4;
    [SerializeField] float spacing = 2f;

    public event Action<Vector3> OnAllArrived;

    int arrivedCount;
    Vector3 lastArrivalPoint;
    Vector3 currentArrivalTarget;

    public List<GameObject> SpawnHorizontal(Vector3 centerPos)
    {
        return SpawnWithOffset(centerPos, Vector3.right);
    }

    public List<GameObject> SpawnVertical(Vector3 centerPos)
    {
        return SpawnWithOffset(centerPos, Vector3.forward);
    }

    List<GameObject> SpawnWithOffset(Vector3 centerPos, Vector3 axis)
    {
        arrivedCount = 0;
        List<GameObject> spawned = new List<GameObject>();

        float totalWidth = spacing * (miniCount - 1);
        Vector3 startPos = centerPos - axis * (totalWidth / 2f);

        for (int i = 0; i < miniCount; i++)
        {
            Vector3 spawnPos = startPos + axis * spacing * i;
            spawnPos.y = 1.0f;

            GameObject mini = Instantiate(miniPrefab, spawnPos, Quaternion.identity);
            ArrivalDetector detector = mini.GetComponent<ArrivalDetector>();

            detector.OnArrived += () => OnMiniArrived();

            spawned.Add(mini);
        }

        return spawned;
    }

    public void OnMiniArrived()
    {
        arrivedCount++;

        if(arrivedCount>=miniCount)
        {
            OnAllArrived?.Invoke(lastArrivalPoint);
        }
    }

    public void SetArrivalTarget(Vector3 target)
    {
        currentArrivalTarget = target;
        arrivedCount = 0;
    }

}
