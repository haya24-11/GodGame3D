using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("éQè∆")]
    [SerializeField] Transform cursorTransform;
    [SerializeField] BigVisibility bigVisibility;
    [SerializeField] MiniSpawner miniSpawner;
    [SerializeField] MiniMoveDispatcher moveDispatcher;
    [SerializeField] ButtonInputReader buttonInput;

    List<GameObject> activeMinis = new List<GameObject>();

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
        if(buttonInput.XButtonDown)
        {
            bigVisibility.Hide();
            activeMinis = miniSpawner.SpawnHorizontal();
        }
        else if(buttonInput.YButtonDown)
        {
            bigVisibility.Hide();
            activeMinis = miniSpawner.SpawnVertical();
        }
    }

    void HandleMoveInput()
    {
        if (activeMinis == null || activeMinis.Count == 0) return;

        Vector3 cursorPos = cursorTransform.position;

        if(buttonInput.RBDown||buttonInput.RTDown)
        {
            moveDispatcher.DispatchAll(activeMinis,cursorPos);
        }
        else if(buttonInput.LBDown||buttonInput.LTDown)
        {
            moveDispatcher.DispatchSequential(activeMinis,cursorPos);
        }
    }

    public void OnAllMinisArrived(Vector3 arrivalPoint)
    {
        activeMinis.Clear();
        bigVisibility.Show(arrivalPoint);
    }

    void OnDestroy()
    {
        miniSpawner.OnAllArrived -= OnAllMinisArrived;
    }
}
