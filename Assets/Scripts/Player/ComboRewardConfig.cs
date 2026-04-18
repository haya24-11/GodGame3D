using UnityEngine;

/// <summary>
/// コンボ段階ごとの報酬値を定義する ScriptableObject。
/// Project ウィンドウで右クリック → Create → ComboSystem → RewardConfig から生成する。
/// ボス戦用・通常用を別々に作成して切り替えることができる。
/// </summary>
[CreateAssetMenu(menuName = "ComboSystem/RewardConfig", fileName = "ComboRewardConfig")]
public class ComboRewardConfig : ScriptableObject
{
    [System.Serializable]
    public struct RewardStep
    {
        [Tooltip("この段階に到達するために必要なヒット数（1始まり）")]
        public int   requiredHits;

        [Tooltip("制限時間への加算値（ボス戦では無視される）")]
        public float timeBonus;

        [Tooltip("ボスへのダメージ加算値（ボス戦のみ有効）")]
        public int   damageBonus;
    }

    [Tooltip("ヒット数に応じた報酬ステップ（requiredHits 昇順で設定すること）")]
    [SerializeField] RewardStep[] steps = new RewardStep[]
    {
        new RewardStep { requiredHits = 1, timeBonus = 0f, damageBonus = 0 },
        new RewardStep { requiredHits = 2, timeBonus = 1f, damageBonus = 1 },
        new RewardStep { requiredHits = 3, timeBonus = 2f, damageBonus = 2 },
        new RewardStep { requiredHits = 4, timeBonus = 3f, damageBonus = 3 },
    };

    /// <summary>
    /// hitCount に対応する RewardStep を返す。
    /// hitCount が最大ステップを超えた場合は最後のステップを返す。
    /// </summary>
    public RewardStep Evaluate(int hitCount)
    {
        RewardStep result = steps[0];
        foreach (var step in steps)
        {
            if (hitCount >= step.requiredHits)
                result = step;
            else
                break;
        }
        return result;
    }
}
