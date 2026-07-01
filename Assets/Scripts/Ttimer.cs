using UnityEngine;
using TMPro;

public class Ttimer : MonoBehaviour
{
    [Header("時間")]
    public int cnttime = 99; // 初期値
    public float interval = 1f;     // 減る間隔（秒）

    [Header("UI")]
    public TextMeshProUGUI timerText;

    private float timer = 0f;
    private bool isTimeOver = false;

    public string nextSceneName; // ← 遷移先（Inspectorで設定）

    //時間追加系？
    public void AddTime(int value)
    {
        cnttime += value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeOver) return;

        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;

            if (cnttime > 0)
            {
                cnttime--;
            }
        }

        if (timerText != null)
        {
            timerText.text = cnttime.ToString("D2");
        }

        if (cnttime <= 0)
        {
            isTimeOver = true;

            StageClearManager manager =
                FindObjectOfType<StageClearManager>();

            if (manager != null)
            {
                manager.OnTimeOver();
            }
            else
            {
                Debug.LogError("[Ttimer] StageClearManager が見つからない");
            }
        }
    }
}
