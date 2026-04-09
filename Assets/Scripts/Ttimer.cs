using UnityEngine;
using TMPro;
public class Ttimer : MonoBehaviour
{

    public int currentValue = 100; // 初期値
    public float interval = 1f;     // 減る間隔（秒）

    private float timer = 0f;
    public TextMeshProUGUI timerText;

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

        // 4桁文字列にする
        string str = currentValue.ToString("D6");

        // 先頭1桁 + ":" + 後ろ3桁
        string formatted = str.Substring(0, 3) + ":" + str.Substring(3, 3);

        timerText.text = formatted; timerText.text = formatted;
    }
}
