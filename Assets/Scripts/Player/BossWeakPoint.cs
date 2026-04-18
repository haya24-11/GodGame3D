using UnityEngine;

/// <summary>
/// ボスにアタッチする弱点コンポーネント。
/// 弱点位置はボスごとにInspectorで設定し、LT押し直しで変わらない。
/// FocusSystemの集中開始/終了に連動して弱点を表示/非表示にする。
/// </summary>
public class BossWeakPoint : MonoBehaviour
{
    [Header("弱点設定")]
    [Tooltip("ボスローカル座標での弱点オフセット")]
    [SerializeField] Vector3 localOffset = Vector3.zero;

    [Tooltip("弱点にヒットした際のダメージ倍率")]
    [SerializeField, Min(1f)] float damageMultiplier = 2f;

    [Tooltip("弱点の当たり判定半径")]
    [SerializeField, Min(0.01f)] float hitRadius = 0.5f;

    [Header("参照")]
    [SerializeField] FocusSystem focusSystem;

    [Tooltip("弱点を示すマーカーオブジェクト（省略可）")]
    [SerializeField] GameObject weakPointMarker;

    // ──────────────────────────────────────────
    // Public properties
    // ──────────────────────────────────────────

    /// <summary>弱点のワールド座標。</summary>
    public Vector3 WorldPosition
        => transform.TransformPoint(localOffset);

    public float DamageMultiplier => damageMultiplier;
    public float HitRadius        => hitRadius;

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void OnEnable()
    {
        if (focusSystem == null) return;
        focusSystem.OnFocusStarted += ShowWeakPoint;
        focusSystem.OnFocusEnded   += HideWeakPoint;

        // 既にフォーカス中なら即表示
        if (focusSystem.IsFocusing) ShowWeakPoint();
        else                        HideWeakPoint();
    }

    void OnDisable()
    {
        if (focusSystem == null) return;
        focusSystem.OnFocusStarted -= ShowWeakPoint;
        focusSystem.OnFocusEnded   -= HideWeakPoint;
    }

    // ──────────────────────────────────────────
    // Public API
    // ──────────────────────────────────────────

    /// <summary>
    /// 攻撃が弱点にヒットしているか判定し、倍率を返す。
    /// ヒットしていない場合は 1f を返す。
    /// </summary>
    public float GetHitMultiplier(Vector3 attackWorldPos)
    {
        if (!focusSystem.IsFocusing) return 1f;
        float dist = Vector3.Distance(attackWorldPos, WorldPosition);
        return dist <= hitRadius ? damageMultiplier : 1f;
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    void ShowWeakPoint()
    {
        if (weakPointMarker != null)
            weakPointMarker.SetActive(true);
    }

    void HideWeakPoint()
    {
        if (weakPointMarker != null)
            weakPointMarker.SetActive(false);
    }

    // ──────────────────────────────────────────
    // Gizmos（Editor確認用）
    // ──────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(WorldPosition, hitRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(WorldPosition, 0.1f);
    }
}
