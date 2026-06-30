using UnityEngine;

/// <summary>
/// フィールド外では Renderer を無効にして非表示にするコンポーネント
/// Box Collider（Trigger）を持つフィールドオブジェクトに対応
/// 敵の動きパターン：
/// - パターン1: フィールド外 → フィールド内 → フィールド外
/// - パターン2: フィールド内 → フィールド外
/// に対応し、フィールド接近時に表示、フィールド外に出て一定時間後に非表示
/// </summary>
public class FieldVisibility : MonoBehaviour, IEnemyComponent
{
    [Header("フィールド設定")]
    [Tooltip("フィールドオブジェクトに設定するタグ名")]
    [SerializeField] private string fieldTag = "Field";

    [Tooltip("初期判定用の検索範囲")]
    [SerializeField] private float initialCheckRadius = 0.5f;

    [Header("非表示のディレイ")]
    [Tooltip("フィールドから出てから非表示になるまでの秒数")]
    [SerializeField] private float hideDelay = 0.1f;

    private Renderer[] renderers;
    private EnemyBaseBeta core;
    private bool isInField = false; // フィールド Trigger 内
    private bool isDisplayed = false; // 実際に表示されている状態
    private float exitTime = 0f; // フィールドを出た時刻

    // ――――――――――――――――――――――――――――――――――――
    // IEnemyComponent
    // ――――――――――――――――――――――――――――――――――――

    public void OnEnemyInit(EnemyBaseBeta core)
    {
        this.core = core;
        renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
        
        // 初期状態でフィールド内にいるかチェック
        CheckInitialFieldStatus();
        exitTime = Time.time;
    }

    // ――――――――――――――――――――――――――――――――――――
    // 初期状態の判定
    // ――――――――――――――――――――――――――――――――――――

    private void CheckInitialFieldStatus()
    {
        // 現在位置がフィールド Trigger 内にあるか確認
        Collider[] overlaps = Physics.OverlapSphere(transform.position, initialCheckRadius);

        isInField = false;
        foreach (var collider in overlaps)
        {
            if (collider.CompareTag(fieldTag) && collider.isTrigger)
            {
                isInField = true;
                break;
            }
        }

        // 初期状態を設定
        isDisplayed = isInField;
    }

    // ――――――――――――――――――――――――――――――――――――
    // 毎フレーム処理
    // ――――――――――――――――――――――――――――――――――――

    private void Update()
    {
        // フィールド内にいるなら常に表示
        if (isInField)
        {
            isDisplayed = true;
        }
        // フィールド外なら、ディレイ時間後に非表示
        else
        {
            float elapsedTime = Time.time - exitTime;
            if (elapsedTime >= hideDelay)
            {
                isDisplayed = false;
            }
        }

        // 実際の表示/非表示を反映
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = isDisplayed;
        }
    }

    // ――――――――――――――――――――――――――――――――――――
    // Trigger 判定
    // ――――――――――――――――――――――――――――――――――――

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(fieldTag))
        {
            isInField = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(fieldTag))
        {
            isInField = false;
            exitTime = Time.time; // フィールドを出た時刻を記録
        }
    }

    // ――――――――――――――――――――――――――――――――――――
    // プール返却時のリセット
    // ――――――――――――――――――――――――――――――――――――

    private void OnDisable()
    {
        if (renderers == null) return;
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = true;
        }
    }
}