// 意図：ボスの移動・ダメージ・死亡処理を自己完結で持つ
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyBossAlpha : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private int maxHp = 10;
    [SerializeField] private float moveSpeed = 4f;

    private int currentHp;

    // ステージ範囲（7:3）
    private float boundX;
    private float boundZ;

    private int currentCorner = 0;
    private Vector3[] corners;

    [SerializeField] private float yHeight = 2f;

    private void Start()
    {
        currentHp = maxHp;

        // カメラから範囲取得
        var cam = Camera.main;
        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        boundX = width;
        boundZ = height;

        // 四隅（時計回り）
        corners = new Vector3[]
        {
            new Vector3(-boundX*0.7f,yHeight,boundZ*0.3f),    // 左上寄り
            new Vector3( boundX*0.7f,yHeight,boundZ*0.3f),    // 右上
            new Vector3( boundX*0.7f,yHeight,-boundZ*0.3f),   // 右下
            new Vector3(-boundX*0.7f,yHeight,-boundZ*0.3f),   // 左下
        };

        // 初期位置
        transform.position = corners[0];
    }

    private void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 target = corners[currentCorner];

        transform.position = Vector3.MoveTowards(
            transform.position,
                target,
                moveSpeed * Time.deltaTime
        );

        // 到達したら次の各へ（時計回り）
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            currentCorner = (currentCorner + 1) % corners.Length;
        }
    }
    
    // ダメージ処理（外部から呼ばれる）
    public void TakeDamage(int damage)
    {
        currentHp -= damage;

        Debug.Log($"Boss Damage: {damage} / HP: {currentHp}");
        // 被弾時：時間＋２
        SendMessage("AddTime", 2, SendMessageOptions.DontRequireReceiver);
        // 被弾時に色が変わる
        StartCoroutine(DamageFlash());

        if(currentHp<=0)
        {
            Die();
        }
    }

    void Die()
    {
        // タイトルへ戻る
        SceneManager.LoadScene("Title");
    }

    // 衝突（mini想定）
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mini"))
        {
            // Mini側に攻撃力持たせてる想定
            int attack = 2;

            var mini = other.GetComponent<MiniAttack>();
            if (mini!=null)
            {
                attack = mini.Attack;
            }
            TakeDamage(attack);
        }
    }

    private Renderer rend;
    private Color originalColor;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    System.Collections.IEnumerator DamageFlash()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = originalColor;
    }
}
