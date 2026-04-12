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
    [SerializeField] float spawnY    = 1.0f;  // ハードコードをInspector公開に

    /// <summary>全MiniがtargetPosに到着したとき、到着地点を引数として発火する。</summary>
    public event Action<Vector3> OnAllArrived;

    int   arrivedCount;
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
        arrivedCount  = 0;
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    List<MiniUnit> SpawnWithOffset(Vector3 centerPos, Vector3 axis)
    {
        arrivedCount = 0;
        var spawned  = new List<MiniUnit>(miniCount);

        float totalWidth = spacing * (miniCount - 1);
        Vector3 startPos = centerPos - axis * (totalWidth / 2f);

        for (int i = 0; i < miniCount; i++)
        {
            Vector3 spawnPos = startPos + axis * spacing * i;
            spawnPos.y = spawnY;

            var mini = Instantiate(miniPrefab, spawnPos, Quaternion.identity);
            var unit = mini.GetComponent<MiniUnit>();

            // スポーン時点で「到着カウント」だけ登録しておく
            unit.Detector.OnArrived += HandleMiniArrived;

            spawned.Add(unit);
        }

        return spawned;
    }

    void HandleMiniArrived()
    {
        arrivedCount++;
        if (arrivedCount >= miniCount)
        {
            OnAllArrived?.Invoke(arrivalTarget);
        }
    }
}
