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
            Vector3 bigPos = bigVisibility.transform.position;
            bigVisibility.Hide();
            activeMinis = miniSpawner.SpawnHorizontal(bigPos);
            canForm = false;
        }
        else if(buttonInput.YButtonDown)
        {
            Vector3 bigPos = bigVisibility.transform.position;
            bigVisibility.Hide();
            activeMinis = miniSpawner.SpawnVertical(bigPos);
            canForm = false;
        }
    }

    void HandleMoveInput()
    {
        if (activeMinis == null || activeMinis.Count == 0) return;

        Vector3 cursorPos = cursorTransform.position;

        if(buttonInput.RBDown||buttonInput.RTDown)
        {
            cursorController.SetMovable(false);
            moveDispatcher.DispatchAll(activeMinis,cursorPos);
        }
        else if(buttonInput.LBDown||buttonInput.LTDown)
        {
            cursorController.SetMovable(false);
            moveDispatcher.DispatchSequential(activeMinis,cursorPos);
        }
    }

    public void OnAllMinisArrived(Vector3 _)
    {
        activeMinis.Clear();

        Vector3 cursorPos = cursorTransform.position;
        bigVisibility.Show(cursorPos);

        cursorController.SetMovable(true);
        canForm = true;
    }

    void OnDestroy()
    {
        miniSpawner.OnAllArrived -= OnAllMinisArrived;
    }
}
