using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MiniMoveDispatcher : MonoBehaviour
{
    [SerializeField] float sequenceInterval = 0.5f;

    public void DispatchAll(List<GameObject> minis,Vector3 target)
    {
        foreach (var mini in minis)
        {
            if (mini == null) continue;

            var mover = mini.GetComponent<Mover>();
            var detector = mini.GetComponent<ArrivalDetector>();

            detector.SetTarget(target);

            detector.OnArrived += () =>
            {
                Destroy(mini);
            };

            mover.SetTargetPosition(target);
        }
    }

    public void DispatchSequential(List<GameObject> minis,Vector3 target)
    {
        var sorted = new List<GameObject>(minis);
        sorted.Sort((a, b) => Vector3.Distance(a.transform.position, target).CompareTo(Vector3.Distance(b.transform.position, target))
        );

        StartCoroutine(SequenceCoroutine(sorted, target));
    }

    IEnumerator SequenceCoroutine(List<GameObject> sorted, Vector3 target)
    {
        foreach (var mini in sorted)
        {
            if (mini == null) continue;

            var mover =mini.GetComponent<Mover>();
            var detector =mini.GetComponent<ArrivalDetector>();

            detector.SetTarget(target);
            detector.OnArrived += () => Destroy(mini);
            mover.SetTargetPosition(target);

            yield return new WaitForSeconds(sequenceInterval);
        }
    }
}
