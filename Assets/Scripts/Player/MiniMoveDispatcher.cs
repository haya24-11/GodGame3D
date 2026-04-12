using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Miniへの移動命令の発行のみを担う。
/// 到着カウントの管理はMiniSpawnerが行うため、このクラスはMiniSpawnerに依存しない。
/// </summary>
public class MiniMoveDispatcher : MonoBehaviour
{
    [SerializeField] float sequenceInterval = 0.5f;

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>全Miniを同時にtargetへ向かわせる。</summary>
    public void DispatchAll(List<MiniUnit> minis, Vector3 target)
    {
        foreach (var unit in minis)
        {
            if (unit == null) continue;
            SendToTarget(unit, target);
        }
    }

    /// <summary>targetに近い順に、intervalごとにMiniを発進させる。</summary>
    public void DispatchSequential(List<MiniUnit> minis, Vector3 target)
    {
        var sorted = new List<MiniUnit>(minis);
        sorted.Sort((a, b) =>
            Vector3.Distance(a.transform.position, target)
            .CompareTo(Vector3.Distance(b.transform.position, target)));

        StartCoroutine(SequenceCoroutine(sorted, target));
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    void SendToTarget(MiniUnit unit, Vector3 target)
    {
        unit.Detector.SetTarget(target);
        unit.Detector.OnArrived += () => Destroy(unit.gameObject);
        unit.Mover.SetTargetPosition(target);
    }

    IEnumerator SequenceCoroutine(List<MiniUnit> sorted, Vector3 target)
    {
        foreach (var unit in sorted)
        {
            if (unit == null) continue;
            SendToTarget(unit, target);
            yield return new WaitForSeconds(sequenceInterval);
        }
    }
}
