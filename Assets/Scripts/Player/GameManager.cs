using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] Transform cursorTransform;
    [SerializeField] BigVisibility bigVisibility;
    [SerializeField] MiniSpawner miniSpawner;
    [SerializeField] MiniMoveDispatcher moveDispatcher;
    [SerializeField] ButtonInputReader buttonInput;
    [SerializeField] CursorController cursorController;

    [SerializeField, Tooltip("ON: Call中もcursorを動かせる / OFF: Call中は固定")]
    bool canMoveCursorDuringCall = false;

    List<GameObject> activeMinis = new List<GameObject>();

    bool canForm = true;
    bool isFormationDeployed = false;
    enum FormationType
    {
        None,
        Horizontal,
        Vertical
    }

    FormationType currentFormation = FormationType.None;

    Vector3 formationOriginPos;

    void Awake()
    {
        miniSpawner.OnAllArrived += OnAllMinisArrived;
    }

    void Update()
    {
        HandleFormationInput();
        HandleMoveInput();
    }

    void HandleFormationInput()
    {
        if (!canForm) return;

        if (buttonInput.XButtonDown)
        {
            HandleFormation(FormationType.Horizontal);
        }
        else if (buttonInput.YButtonDown)
        {
            HandleFormation(FormationType.Vertical);
        }

    }

    void HandleMoveInput()
    {
        if (activeMinis == null || activeMinis.Count == 0) return;

        Vector3 cursorPos = cursorTransform.position;

        if(buttonInput.RBDown||buttonInput.RTDown)
        {
            OnCallStarted(cursorPos, isSequential: false);
        }
        else if(buttonInput.LBDown||buttonInput.LTDown)
        {
            OnCallStarted(cursorPos, isSequential: true);
        }
    }

    public void OnAllMinisArrived(Vector3 _)
    {
        activeMinis.Clear();

        Vector3 cursorPos = cursorTransform.position;
        bigVisibility.Show(cursorPos);

        cursorController.SetMovable(true);
        canForm = true;

        isFormationDeployed = false;
        currentFormation = FormationType.None;
    }

    void OnDestroy()
    {
        miniSpawner.OnAllArrived -= OnAllMinisArrived;
    }

    void HandleFormation(FormationType type)
    {

        if (isFormationDeployed)
        {
            if (currentFormation == type)
            {
                CancelFormation();
            }
            return;
        }

        formationOriginPos = bigVisibility.transform.position;

        bigVisibility.Hide();

        switch (type)
        {
            case FormationType.Horizontal:
                activeMinis = miniSpawner.SpawnHorizontal(formationOriginPos);
                break;

            case FormationType.Vertical:
                activeMinis = miniSpawner.SpawnVertical(formationOriginPos);
                break;
        }

        currentFormation = type;
        isFormationDeployed = true;
    }

    void CancelFormation()
    {
        foreach (var mini in activeMinis)
        {
            if (mini != null)
                Destroy(mini);
        }
        activeMinis.Clear();

        bigVisibility.Show(formationOriginPos);

        isFormationDeployed = false;
        currentFormation = FormationType.None;
    }
    void OnCallStarted(Vector3 targetPos, bool isSequential)
    {
        canForm = false;

        if (!canMoveCursorDuringCall)
        {
            cursorController.SetMovable(false);
        }

        if (isSequential)
        {
            moveDispatcher.DispatchSequential(activeMinis, targetPos);
        }
        else
        {
            moveDispatcher.DispatchAll(activeMinis, targetPos);
        }
    }
}
