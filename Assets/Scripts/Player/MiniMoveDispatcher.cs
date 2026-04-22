using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Miniへの移動命令の発行のみを担う。
/// 到着カウントの管理はMiniSpawnerが行うため、このクラスはMiniSpawnerに依存しない。
/// </summary>
public class MiniMoveDispatcher : MonoBehaviour
{
    [SerializeField] float sequenceInterval = 0.5f;

    [Header("コンボシステム")]
    [SerializeField] ComboCounter comboCounter;

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
        // ラムダを変数に持つことで -= による除去を可能にし、二重登録を防ぐ
        Action onArrived = null;
        onArrived = () =>
        {
            unit.Detector.OnArrived -= onArrived;   // 自身を解除

            comboCounter?.RegisterHit();
            Destroy(unit.gameObject);
        };

        unit.Detector.OnArrived += onArrived;
        unit.Detector.SetTarget(target);
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
