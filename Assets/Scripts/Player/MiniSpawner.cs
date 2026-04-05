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

    public List<GameObject> SpawnHorizontal()
    {
        return SpawnWithOffset(Vector3.right);
    }

    public List<GameObject> SpawnVertical()
    {
        return SpawnWithOffset(Vector3.forward);
    }

    List<GameObject> SpawnWithOffset(Vector3 axis)
    {
        arrivedCount = 0;
        var spawned = new List<GameObject>();

        float totalWidth = spacing * (miniCount - 1);
        Vector3 startPos = transform.position - axis * (totalWidth / 2f);

        for (int i =0; i<miniCount;i++)
        {
            Vector3 spawnPos = startPos + axis * spacing * i;
            spawnPos.y = 1.0f;

            var mini = Instantiate(miniPrefab, spawnPos, Quaternion.identity);
            var detector = mini.GetComponent<ArrivalDetector>();

            detector.OnArrived += () => OnMiniArrived(spawnPos);

            spawned.Add(mini);
        }

        return spawned;
    }

    void OnMiniArrived(Vector3 arrivalPos)
    {
        lastArrivalPoint= arrivalPos;
        arrivedCount++;

        if(arrivedCount>=miniCount)
        {
            OnAllArrived?.Invoke(lastArrivalPoint);
        }
    }
}
