using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMoveDispatcher : MonoBehaviour
{
    [SerializeField] MiniSpawner miniSpawner;
    [SerializeField] float sequenceInterval = 0.5f;

    public void DispatchAll(List<GameObject> minis,Vector3 target)
    {
        miniSpawner.SetArrivalTarget(target);

        foreach (GameObject mini in minis)
        {
            if (mini == null) continue;

            Mover mover = mini.GetComponent<Mover>();
            ArrivalDetector detector = mini.GetComponent<ArrivalDetector>();

            detector.SetTarget(target);

            detector.OnArrived += () =>
            {
                Destroy(mini);
                miniSpawner.OnMiniArrived();
            };

            mover.SetTargetPosition(target);
        }
    }

    public void DispatchSequential(List<GameObject> minis,Vector3 target)
    {
        List<GameObject> sorted = new List<GameObject>(minis);
        sorted.Sort((a, b) => Vector3.Distance(a.transform.position, target).CompareTo(Vector3.Distance(b.transform.position, target))
        );

        StartCoroutine(SequenceCoroutine(sorted, target));
    }

    IEnumerator SequenceCoroutine(List<GameObject> sorted, Vector3 target)
    {
        foreach (GameObject mini in sorted)
        {
            if (mini == null) continue;

            Mover mover =mini.GetComponent<Mover>();
            ArrivalDetector detector =mini.GetComponent<ArrivalDetector>();

            detector.SetTarget(target);
            detector.OnArrived += () => Destroy(mini);
            mover.SetTargetPosition(target);

            yield return new WaitForSeconds(sequenceInterval);
        }
    }
}
