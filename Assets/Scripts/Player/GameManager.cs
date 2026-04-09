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
            cursorController.SetMovable(false);
            canForm = false;
            moveDispatcher.DispatchAll(activeMinis,cursorPos);
        }
        else if(buttonInput.LBDown||buttonInput.LTDown)
        {
            cursorController.SetMovable(false);
            canForm = false;
            moveDispatcher.DispatchSequential(activeMinis,cursorPos);
        }
    }

    public void OnAllMinisArrived(Vector3 _)
    {
        activeMinis.Clear();

        Vector3 cursorPos = cursorTransform.position;
        bigVisibility.Show(cursorPos);

        isFormationDeployed = false;
        currentFormation = FormationType.None;
        canForm = true;

        cursorController.SetMovable(true);
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
}
