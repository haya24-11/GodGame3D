using UnityEngine;
using System;

/// <summary>
/// 集中システムの中枢。LT押下中かどうかを管理し、
/// 各サブシステムへイベントで通知する。
/// GameManagerのLT判定とは独立して動作する。
/// </summary>
public class FocusSystem : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] ButtonInputReader buttonInput;

    [Header("集中中の速度倍率")]
    [SerializeField, Tooltip("制限時間消費速度の倍率")]
    float timeConsumptionMultiplier = 1.5f;

    [SerializeField, Tooltip("MiniおよびCursorの移動速度倍率")]
    float moveSpeedMultiplier = 1.5f;

    // ──────────────────────────────────────────
    // Public properties
    // ──────────────────────────────────────────

    public bool  IsFocusing              { get; private set; }
    public float TimeConsumptionMultiplier => timeConsumptionMultiplier;
    public float MoveSpeedMultiplier       => moveSpeedMultiplier;

    // ──────────────────────────────────────────
    // Events
    // ──────────────────────────────────────────

    /// <summary>集中開始時に発火。</summary>
    public event Action OnFocusStarted;

    /// <summary>集中終了時に発火。</summary>
    public event Action OnFocusEnded;

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void Update()
    {
        bool ltHeld = buttonInput != null
                      && Gamepad_LTHeld();

        if (ltHeld && !IsFocusing)
        {
            IsFocusing = true;
            OnFocusStarted?.Invoke();
        }
        else if (!ltHeld && IsFocusing)
        {
            IsFocusing = false;
            OnFocusEnded?.Invoke();
        }
    }

    // ──────────────────────────────────────────
    // Private
    // ──────────────────────────────────────────

    /// <summary>
    /// ButtonInputReaderはwasPressedThisFrameのみ公開しているため、
    /// LTの「押し続け」はInputSystem経由で直接読む。
    /// </summary>
    bool Gamepad_LTHeld()
    {
        var gp = UnityEngine.InputSystem.Gamepad.current;
        if (gp == null) return false;
        return gp.leftTrigger.isPressed;
    }
}
