using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] Transform         cursorTransform;
    [SerializeField] BigVisibility     bigVisibility;
    [SerializeField] MiniSpawner       miniSpawner;
    [SerializeField] MiniMoveDispatcher moveDispatcher;
    [SerializeField] ButtonInputReader  buttonInput;
    [SerializeField] CursorController   cursorController;
    [SerializeField] ComboCounter comboCounter;

    [SerializeField, Tooltip("ON: Call中もcursorを動かせる / OFF: Call中は固定")]
    bool canMoveCursorDuringCall = false;

    // ──────────────────────────────────────────
    // State
    // ──────────────────────────────────────────

    List<MiniUnit> activeMinis     = new List<MiniUnit>();
    bool           canForm         = true;
    bool           isFormationDeployed = false;
    FormationType  currentFormation    = FormationType.None;
    Vector3        formationOriginPos;

    // ──────────────────────────────────────────
    // Unity lifecycle
    // ──────────────────────────────────────────

    void Awake()
    {
        miniSpawner.OnAllArrived += OnAllMinisArrived;
    }

    void OnDestroy()
    {
        miniSpawner.OnAllArrived -= OnAllMinisArrived;
    }

    void Update()
    {
        HandleFormationInput();
        HandleMoveInput();
    }

    // ──────────────────────────────────────────
    // Input handlers
    // ──────────────────────────────────────────

    void HandleFormationInput()
    {
        if (!canForm) return;

        if      (buttonInput.XButtonDown) HandleFormation(FormationType.Horizontal);
        else if (buttonInput.YButtonDown) HandleFormation(FormationType.Vertical);
    }

    void HandleMoveInput()
    {
        if (activeMinis == null || activeMinis.Count == 0) return;

        Vector3 cursorPos = cursorTransform.position;

        if (buttonInput.RBDown) StartCall(cursorPos, isSequential: false);
        else if (buttonInput.LBDown) StartCall(cursorPos, isSequential: true);
    }

    // ──────────────────────────────────────────
    // Formation
    // ──────────────────────────────────────────

    void HandleFormation(FormationType type)
    {
        if (isFormationDeployed)
        {
            // 同じ隊形ボタンを再押しでキャンセル
            if (currentFormation == type) CancelFormation();
            return;
        }

        formationOriginPos = bigVisibility.transform.position;
        bigVisibility.Hide();

        activeMinis = type switch
        {
            FormationType.Horizontal => miniSpawner.SpawnHorizontal(formationOriginPos),
            FormationType.Vertical   => miniSpawner.SpawnVertical(formationOriginPos),
            _                        => activeMinis
        };

        currentFormation   = type;
        isFormationDeployed = true;
    }

    void CancelFormation()
    {
        foreach (var unit in activeMinis)
        {
            if (unit != null) Destroy(unit.gameObject);
        }
        activeMinis.Clear();

        bigVisibility.Show(formationOriginPos);

        isFormationDeployed = false;
        currentFormation    = FormationType.None;
    }

    // ──────────────────────────────────────────
    // Call (移動命令)
    // ──────────────────────────────────────────

    void StartCall(Vector3 targetPos, bool isSequential)
    {
        canForm = false;

        if (!canMoveCursorDuringCall)
            cursorController.SetMovable(false);

        comboCounter?.BeginCall();

        // カウンタ・ターゲットをMiniSpawnerに通知してからDispatch
        miniSpawner.PrepareForDispatch(targetPos);

        if (isSequential)
            moveDispatcher.DispatchSequential(activeMinis, targetPos);
        else
            moveDispatcher.DispatchAll(activeMinis, targetPos);
    }

    // ──────────────────────────────────────────
    // Callbacks
    // ──────────────────────────────────────────

    void OnAllMinisArrived(Vector3 _)
    {
        activeMinis.Clear();

        bigVisibility.Show(cursorTransform.position);

        cursorController.SetMovable(true);
        canForm             = true;
        isFormationDeployed = false;
        currentFormation    = FormationType.None;

        comboCounter?.EndCall();
    }
}
