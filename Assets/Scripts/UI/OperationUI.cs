// どのファイルのどこを変更：OperationUI.cs（新規）
// 意図：操作説明をUIに表示する

using UnityEngine;
using TMPro;

public class OperationUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        text.text =
@"
LSTICK : MOVE
         Y : FORCE
    → Y : RESET
        R : CALL";
    }
}