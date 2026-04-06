using UnityEngine;

public class CursorController : MonoBehaviour
{

    [SerializeField] InputReader inputReader;
    [SerializeField] Mover mover;

    public bool CanMove { get; private set; } = true;

    public void SetMovable(bool movable)
    {
        CanMove = movable;
    }


    void Update()
    {
        if (!CanMove) return;

        Vector2 input = inputReader.MoveInput;

        mover.MoveFromInput(input.normalized);
    }
}
