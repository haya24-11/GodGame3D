using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class Ttimer : MonoBehaviour
{

    public int currentValue = 100; // 初期値
    public float interval = 1f;     // 減る間隔（秒）

    private float timer = 0f;
    public TextMeshProUGUI timerText;

    public string nextSceneName; // ← 遷移先（Inspectorで設定）
    private bool isFinished = false; // ← 1回だけ実行するため

    //時間追加系？
    public void AddTime(int value)
    {
        currentValue += value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            timer = 0f;

            if (currentValue > 0)
            {
                currentValue--;
            }
        }

        // 3桁文字列にする
        string str = currentValue.ToString("D3");

        // 先頭1桁 + ":" + 後ろ3桁
        string formatted = str.Substring(0, 3);// + ":" + str.Substring(3, 3);

        timerText.text = formatted; timerText.text = formatted;

        if (currentValue == 0)
        {
            isFinished = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
