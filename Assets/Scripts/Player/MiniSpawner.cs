using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Miniのスポーンと、全Mini到着の検知を担う。
/// 到着カウントの管理はここに一元化し、MiniMoveDispatcherは関与しない。
/// </summary>
public class MiniSpawner : MonoBehaviour
{
    [SerializeField] GameObject miniPrefab;
    [SerializeField] int   miniCount = 4;
    [SerializeField] float spacing   = 2f;
    [SerializeField] float spawnY    = 1.0f;

    [Header("集中システム")]
    [SerializeField] FocusSystem focusSystem;

    /// <summary>全MiniがtargetPosに到着したとき、到着地点を引数として発火する。</summary>
    public event Action<Vector3> OnAllArrived;

    int     pendingCount;
    Vector3 arrivalTarget;

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    public List<MiniUnit> SpawnHorizontal(Vector3 centerPos)
        => SpawnWithOffset(centerPos, Vector3.right);

    public List<MiniUnit> SpawnVertical(Vector3 centerPos)
        => SpawnWithOffset(centerPos, Vector3.forward);

    /// <summary>
    /// Dispatchが始まる前に呼んでカウンタとターゲットをリセットする。
    /// </summary>
    public void PrepareForDispatch(Vector3 target)
    {
        arrivalTarget = target;
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    List<MiniUnit> SpawnWithOffset(Vector3 centerPos, Vector3 axis)
    {
        var spawned  = new List<MiniUnit>(miniCount);

        float totalWidth = spacing * (miniCount - 1);
        Vector3 startPos = centerPos - axis * (totalWidth / 2f);

        for (int i = 0; i < miniCount; i++)
        {
            Vector3 spawnPos = startPos + axis * spacing * i;
            spawnPos.y = spawnY;

            var mini = Instantiate(miniPrefab, spawnPos, Quaternion.identity);
            var unit = mini.GetComponent<MiniUnit>();

            // 集中システムが設定されていればFocusSpeedModifierを動的に付与
            if (focusSystem != null)
            {
                var speedMod = mini.AddComponent<FocusSpeedModifier>();
                speedMod.InjectFocusSystem(focusSystem);
            }

            // スポーン時点で「到着カウント」だけ登録しておく
            unit.Detector.OnArrived += HandleMiniArrived;

            spawned.Add(unit);
        }
        pendingCount = spawned.Count;

        return spawned;
    }

    void HandleMiniArrived()
    {
        pendingCount--;
        if (pendingCount <= 0)
        {
            pendingCount = 0;
            OnAllArrived?.Invoke(arrivalTarget);
        }
    }
}
