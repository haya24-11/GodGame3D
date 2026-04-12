using UnityEngine;

/// <summary>
/// Miniのコンポーネント参照をキャッシュする。
/// MoverとArrivalDetectorへのGetComponent呼び出しをDispatcher側で毎回行わずに済む。
/// </summary>
[RequireComponent(typeof(Mover))]
[RequireComponent(typeof(ArrivalDetector))]
public class MiniUnit : MonoBehaviour
{
    public Mover        Mover    { get; private set; }
    public ArrivalDetector Detector { get; private set; }

    void Awake()
    {
        Mover    = GetComponent<Mover>();
        Detector = GetComponent<ArrivalDetector>();
    }
}
