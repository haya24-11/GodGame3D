using System.Collections;
using UnityEngine;

/// <summary>
/// ゲーム内エフェクトの再生を一元管理するシングルトンクラス。
/// </summary>
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    // ----------------------------------------
    // Prefab参照（Inspectorでアサインする）
    // ----------------------------------------

    [Header("2Dエフェクト")]
    [SerializeField] private GameObject effectFocusPrefab;       // effect_focus
    [SerializeField] private GameObject effectBossAlartPrefab;   // effect_boss_alart
    [SerializeField] private GameObject effectTimePlusPrefab;    // effect_timePlus
    [SerializeField] private GameObject effectReadyPrefab;       // effect_Ready
    [SerializeField] private GameObject effectGoPrefab;          // effect_Go

    [Header("3Dエフェクト")]
    [SerializeField] private GameObject effectCallElectronicPrefab;    // effect_call_electronic
    [SerializeField] private GameObject effectEnemyDeathPrefab;        // effect_enemy_death
    [SerializeField] private GameObject effectWeekpointPrefab;         // effect_weekpoint
    [SerializeField] private GameObject effectTrideraUnitDeathPrefab;  // effect_trideraunit_death
    [SerializeField] private GameObject effectRamNeedleSpeedPrefab;    // effect_ramNeedle_speed（ループ）
    [SerializeField] private GameObject effectPomoonDeathPrefab;       // effect_pomoon_death
    [SerializeField] private GameObject effectBossLandingPrefab;       // effect_boss_landing
    [SerializeField] private GameObject effectBossSpeedPrefab;         // effect_boss_speed（ループ）

    [Header("UI")]
    [SerializeField] private GameObject effectControllerPrefab; // effect_controller（PAUSE画面操作説明）

    // ----------------------------------------
    // ループエフェクトの実体参照（Start/Stop管理）
    // ----------------------------------------
    private GameObject activeFocusEffect;
    private GameObject activeRamNeedleSpeedEffect;
    private GameObject activeBossSpeedEffect;
    private GameObject activeControllerUI;

    // ----------------------------------------
    // 定数
    // ----------------------------------------

    /// <summary>集中モードのフェード時間（秒）</summary>
    private const float FOCUS_FADE_DURATION = 0.1f;

    /// <summary>
    /// GAMESTART演出のReady表示時間（秒）。マスター版で要調整。
    /// </summary>
    [Header("GameStart調整値（マスター版で要調整）")]
    [SerializeField] private float readyDuration = 1.0f;

    /// <summary>
    /// GAMESTART演出のGoフェードアウト時間（秒）。マスター版で要調整。
    /// </summary>
    [SerializeField] private float goFadeDuration = 0.5f;

    /// <summary>
    /// 敵の右上オフセット（timePlus表示位置）
    /// </summary>
    private static readonly Vector3 TIME_PLUS_OFFSET = new Vector3(0.5f, 0.5f, 0f);

    // ========================================
    // 初期化
    // ========================================

    void Awake()
    {
        // シングルトン（シーン遷移をまたいで保持）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========================================
    // 共通関数
    // ========================================

    /// <summary>
    /// 指定Prefabを指定座標にInstantiateし、destroyTime秒後に自動Destroyする汎用メソッド。
    /// </summary>
    public void PlayEffect(GameObject effectPrefab, Vector3 position, float destroyTime = 2f)
    {
        if (effectPrefab == null)
        {
            Debug.LogWarning($"[EffectManager] PlayEffect: effectPrefabがnullです。Inspectorでアサインしてください。");
            return;
        }
        GameObject fx = Instantiate(effectPrefab, position, Quaternion.identity);
        Destroy(fx, destroyTime);
    }

    // ========================================
    // 2Dエフェクト
    // ========================================

    // ----------------------------------------
    // 1. 集中モード使用中エフェクト（effect_focus）
    //    ・集中モードボタン押下時: PlayFocusEffect()
    //    ・集中モードボタン離す時: StopFocusEffect()
    //    ・フェードイン/アウト 0.1秒、透明度 0 ↔ 1
    //    ・ループ再生（Destroy不要）
    // ----------------------------------------

    /// <summary>集中モード開始時に呼び出す。フェードインしてループ再生する。</summary>
    public void PlayFocusEffect()
    {
        if (activeFocusEffect != null) return; // 二重起動防止
        if (effectFocusPrefab == null) { Debug.LogWarning("[EffectManager] effectFocusPrefab未アサイン"); return; }

        activeFocusEffect = Instantiate(effectFocusPrefab, Vector3.zero, Quaternion.identity);
        SetAlpha(activeFocusEffect, 0f);
        StartCoroutine(FadeEffect(activeFocusEffect, 0f, 1f, FOCUS_FADE_DURATION));
    }

    /// <summary>集中モード終了時に呼び出す。フェードアウト後にDestroyする。</summary>
    public void StopFocusEffect()
    {
        if (activeFocusEffect == null) return;
        GameObject target = activeFocusEffect;
        activeFocusEffect = null;
        StartCoroutine(FadeAndDestroy(target, 1f, 0f, FOCUS_FADE_DURATION));
    }

    // ----------------------------------------
    // 2. ボス出現時警告（effect_boss_alart）
    //    ・ボス出現前に呼び出す
    //    ・発生時間 2秒程度
    // ----------------------------------------

    /// <summary>ボス出現前の警告演出を再生する。再生完了後にボスを出現させること。</summary>
    public void PlayBossAlart(Vector3 position)
    {
        PlayEffect(effectBossAlartPrefab, position, 2f);
    }

    // ----------------------------------------
    // 3. 敵撃破時の時間追加エフェクト（effect_timePlus）
    //    ・敵の右上に表示
    //    ・画像素材内に字は不要（時間の値は動的にオーバーレイする）
    // ----------------------------------------

    /// <summary>敵撃破で時間が増えた際、enemyPositionの右上にエフェクトを表示する。</summary>
    /// <param name="enemyPosition">敵オブジェクトのワールド座標</param>
    public void PlayTimePlus(Vector3 enemyPosition)
    {
        PlayEffect(effectTimePlusPrefab, enemyPosition + TIME_PLUS_OFFSET, 1.5f);
    }

    // ----------------------------------------
    // 4. GAMESTART演出（effect_Ready / effect_Go）
    //    ・ゲーム開始時にTIMEを止めた状態で呼び出す
    //    ・Ready → Go のフロー
    //    ・GoはフェードアウトIN(透明度100→0)
    //    ・readyDuration / goFadeDuration はInspectorで調整
    // ----------------------------------------

    /// <summary>ゲーム開始時の演出を再生する。完了コールバックでTIMEを再開すること。</summary>
    /// <param name="onComplete">演出完了時に呼び出すコールバック（任意）</param>
    public void PlayGameStart(System.Action onComplete = null)
    {
        StartCoroutine(GameStartSequence(onComplete));
    }

    private IEnumerator GameStartSequence(System.Action onComplete)
    {
        // Ready表示
        if (effectReadyPrefab != null)
        {
            GameObject ready = Instantiate(effectReadyPrefab, Vector3.zero, Quaternion.identity);
            yield return new WaitForSeconds(readyDuration);
            Destroy(ready);
        }

        // Go表示→フェードアウト
        if (effectGoPrefab != null)
        {
            GameObject go = Instantiate(effectGoPrefab, Vector3.zero, Quaternion.identity);
            SetAlpha(go, 1f);
            yield return StartCoroutine(FadeAndDestroy(go, 1f, 0f, goFadeDuration));
        }

        onComplete?.Invoke();
    }

    // ========================================
    // 3Dエフェクト
    // ========================================

    // ----------------------------------------
    // 5. Call押下時のcursorエフェクト（effect_call_electronic）
    //    ・発生時間 0.25秒程度
    //    ・電気的な印象、派手さ控えめ
    // ----------------------------------------

    /// <summary>Callボタン押下時にcursor位置でエフェクトを発生させる。</summary>
    public void PlayCallElectronic(Vector3 position)
    {
        PlayEffect(effectCallElectronicPrefab, position, 0.25f);
    }

    // ----------------------------------------
    // 6. 敵撃破エフェクト（effect_enemy_death）
    //    ・撃破アニメーションへ移行するフレームに発生
    //    ・発生時間 0.25秒程度
    // ----------------------------------------

    /// <summary>enemyオブジェクト撃破時に呼び出す。撃破アニメーション移行フレームで発生させること。</summary>
    public void PlayEnemyDeath(Vector3 position)
    {
        PlayEffect(effectEnemyDeathPrefab, position, 0.25f);
    }

    // ----------------------------------------
    // 7. 弱点攻撃エフェクト（effect_weekpoint）
    //    ・ボスの弱点に攻撃が当たった際
    //    ・発生時間 0.2秒程度
    // ----------------------------------------

    /// <summary>ボスの弱点ヒット時に呼び出す。</summary>
    public void PlayWeekpoint(Vector3 position)
    {
        PlayEffect(effectWeekpointPrefab, position, 0.2f);
    }

    // ----------------------------------------
    // 8. トリデラのUnit撃破エフェクト（effect_trideraunit_death）
    //    ・Unitが撃破されるアニメーションへ移行するフレームに発生
    //    ・発生時間 0.2秒程度
    // ----------------------------------------

    /// <summary>トリデラのUnit撃破時に呼び出す。</summary>
    public void PlayTrideraUnitDeath(Vector3 position)
    {
        PlayEffect(effectTrideraUnitDeathPrefab, position, 0.2f);
    }

    // ----------------------------------------
    // 9. ラムニードル加速エフェクト（effect_ramNeedle_speed）
    //    ・加速中、ラムニードルに常に追従して発生（ループ）
    //    ・発生時間 0.2秒でループ再生
    // ----------------------------------------

    /// <summary>ラムニードル加速開始時に呼び出す。parentのTransformに追従する。</summary>
    public void StartRamNeedleSpeed(Transform parent)
    {
        if (activeRamNeedleSpeedEffect != null) return;
        if (effectRamNeedleSpeedPrefab == null) { Debug.LogWarning("[EffectManager] effectRamNeedleSpeedPrefab未アサイン"); return; }
        activeRamNeedleSpeedEffect = Instantiate(effectRamNeedleSpeedPrefab, parent.position, Quaternion.identity, parent);
    }

    /// <summary>ラムニードル加速終了時に呼び出す。</summary>
    public void StopRamNeedleSpeed()
    {
        if (activeRamNeedleSpeedEffect == null) return;
        Destroy(activeRamNeedleSpeedEffect);
        activeRamNeedleSpeedEffect = null;
    }

    // ----------------------------------------
    // 10. ポムーン爆発（effect_pomoon_death）
    //     ・画面内の敵を全滅させる大きめのエフェクト
    //     ・発生時間 0.2秒程度
    // ----------------------------------------

    /// <summary>ポムーン撃破時に呼び出す。</summary>
    public void PlayPomoonDeath(Vector3 position)
    {
        PlayEffect(effectPomoonDeathPrefab, position, 0.2f);
    }

    // ----------------------------------------
    // 11. ボス着地（effect_boss_landing）
    //     ・ボスが画面内に出現した際のエフェクト
    //     ・発生時間 0.2秒程度
    // ----------------------------------------

    /// <summary>ボスが画面内に出現した際に呼び出す。</summary>
    public void PlayBossLanding(Vector3 position)
    {
        PlayEffect(effectBossLandingPrefab, position, 0.2f);
    }

    // ----------------------------------------
    // 12. ワイパー/ミメシス加速エフェクト（effect_boss_speed）
    //     ・突進攻撃中に常に追従して表示（ループ）
    //     ・発生時間 0.2秒でループ再生
    // ----------------------------------------

    /// <summary>ワイパー/ミメシスの突進開始時に呼び出す。parentのTransformに追従する。</summary>
    public void StartBossSpeed(Transform parent)
    {
        if (activeBossSpeedEffect != null) return;
        if (effectBossSpeedPrefab == null) { Debug.LogWarning("[EffectManager] effectBossSpeedPrefab未アサイン"); return; }
        activeBossSpeedEffect = Instantiate(effectBossSpeedPrefab, parent.position, Quaternion.identity, parent);
    }

    /// <summary>ワイパー/ミメシスの突進終了時に呼び出す。</summary>
    public void StopBossSpeed()
    {
        if (activeBossSpeedEffect == null) return;
        Destroy(activeBossSpeedEffect);
        activeBossSpeedEffect = null;
    }

    // ========================================
    // UI
    // ========================================

    // ----------------------------------------
    // 13. 操作説明画像（effect_controller）
    //     ・PAUSE画面で表示/非表示を切り替える
    // ----------------------------------------

    /// <summary>PAUSE画面を開いた際に操作説明画像を表示する。</summary>
    public void ShowController()
    {
        if (activeControllerUI != null) return;
        if (effectControllerPrefab == null) { Debug.LogWarning("[EffectManager] effectControllerPrefab未アサイン"); return; }
        activeControllerUI = Instantiate(effectControllerPrefab);
    }

    /// <summary>PAUSE画面を閉じた際に操作説明画像を非表示にする。</summary>
    public void HideController()
    {
        if (activeControllerUI == null) return;
        Destroy(activeControllerUI);
        activeControllerUI = null;
    }

    // ========================================
    // フェードユーティリティ
    // ========================================

    /// <summary>
    /// CanvasGroup または SpriteRenderer のアルファ値を瞬時にセットする。
    /// </summary>
    private void SetAlpha(GameObject target, float alpha)
    {
        if (target == null) return;
        var cg = target.GetComponent<CanvasGroup>();
        if (cg != null) { cg.alpha = alpha; return; }
        var sr = target.GetComponent<SpriteRenderer>();
        if (sr != null) { Color c = sr.color; c.a = alpha; sr.color = c; }
    }

    /// <summary>
    /// targetのアルファ値を startAlpha → endAlpha へ duration秒かけて変化させる。
    /// CanvasGroup / SpriteRenderer の両方に対応。
    /// </summary>
    private IEnumerator FadeEffect(GameObject target, float startAlpha, float endAlpha, float duration)
    {
        if (target == null) yield break;
        var cg = target.GetComponent<CanvasGroup>();
        var sr = target.GetComponent<SpriteRenderer>();

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (target == null) yield break;
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            if (cg != null) cg.alpha = alpha;
            if (sr != null) { Color c = sr.color; c.a = alpha; sr.color = c; }
            yield return null;
        }
    }

    /// <summary>
    /// フェード完了後にtargetをDestroyする。
    /// </summary>
    private IEnumerator FadeAndDestroy(GameObject target, float startAlpha, float endAlpha, float duration)
    {
        yield return StartCoroutine(FadeEffect(target, startAlpha, endAlpha, duration));
        if (target != null) Destroy(target);
    }
}
