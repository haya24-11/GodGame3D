using UnityEngine;

/// <summary>
/// ゲームの最初のシーンに置く初期化用オブジェクト。
/// SceneHistory シングルトンをここで生成します。
/// </summary>
public class SceneHistoryInitializer : MonoBehaviour
{
    void Awake()
    {
        // すでに存在する場合は生成しない
        if (SceneHistory.Instance != null) return;

        var go = new GameObject("SceneHistory");
        go.AddComponent<SceneHistory>();
    }
}