using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ImageSpawnerSelector : MonoBehaviour
{
    public enum SelectAction
    {
        LoadScene,
        QuitGame
    }

    [System.Serializable]
    public class ImageData
    {
        [Header("表示位置(XYZ)")]
        public Vector3 position;

        [Header("画像サイズ(XY)")]
        public Vector2 size;

        [Header("回転角度(Z)")]
        public float angle;

        [Header("通常画像")]
        public Sprite normalSprite;

        [Header("選択画像")]
        public Sprite selectedSprite;

        [Header("決定時の動作")]
        public SelectAction action;

        [Header("遷移先シーン名")]
        public string sceneName;
    }

    [Header("生成先Canvas")]
    public Canvas canvas;

    [Header("生成する画像一覧")]
    public ImageData[] images;

    private List<Image> spawnedImages = new List<Image>();

    private int currentIndex = 0;

    private InputAction navigateAction;
    private InputAction submitAction;

    // スティック倒しっぱなし対策
    private bool stickHeld = false;

    private void Awake()
    {
        navigateAction = new InputAction(
            name: "Navigate",
            type: InputActionType.Value,
            binding: "<Gamepad>/dpad"
        );

        // 左スティックでも操作可能
        navigateAction.AddBinding("<Gamepad>/leftStick");

        submitAction = new InputAction(
            name: "Submit",
            type: InputActionType.Button,
            binding: "<Gamepad>/buttonSouth"
        );
    }

    private void OnEnable()
    {
        navigateAction.Enable();
        submitAction.Enable();

        navigateAction.performed += OnNavigate;
        submitAction.performed += OnSubmit;
    }

    private void OnDisable()
    {
        navigateAction.performed -= OnNavigate;
        submitAction.performed -= OnSubmit;

        navigateAction.Disable();
        submitAction.Disable();
    }

    private void Start()
    {
        foreach (ImageData data in images)
        {
            CreateImage(data);
        }

        UpdateSelection();
    }

    private void Update()
    {
        Vector2 input = navigateAction.ReadValue<Vector2>();

        // スティックが中央に戻ったら再入力可能
        if (Mathf.Abs(input.y) < 0.3f)
        {
            stickHeld = false;
        }
    }

    private void CreateImage(ImageData data)
    {
        GameObject obj = new GameObject("MenuImage");

        obj.transform.SetParent(canvas.transform, false);

        Image img = obj.AddComponent<Image>();
        img.sprite = data.normalSprite;

        RectTransform rect = img.rectTransform;

        rect.anchoredPosition3D = data.position;
        rect.sizeDelta = data.size;
        rect.localEulerAngles = new Vector3(0, 0, data.angle);

        spawnedImages.Add(img);
    }

    private void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (stickHeld)
            return;

        if (input.y > 0.5f)
        {
            currentIndex--;

            if (currentIndex < 0)
                currentIndex = images.Length - 1;

            UpdateSelection();

            stickHeld = true;
        }
        else if (input.y < -0.5f)
        {
            currentIndex++;

            if (currentIndex >= images.Length)
                currentIndex = 0;

            UpdateSelection();

            stickHeld = true;
        }
    }

    private void OnSubmit(InputAction.CallbackContext context)
    {
        ImageData selected = images[currentIndex];

        switch (selected.action)
        {
            case SelectAction.LoadScene:

                if (!string.IsNullOrEmpty(selected.sceneName))
                {
                    SceneManager.LoadScene(selected.sceneName);
                }
                else
                {
                    Debug.LogWarning("シーン名が設定されていません");
                }

                break;

            case SelectAction.QuitGame:

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif

                break;
        }
    }

    private void UpdateSelection()
    {
        for (int i = 0; i < spawnedImages.Count; i++)
        {
            if (i == currentIndex)
            {
                spawnedImages[i].sprite = images[i].selectedSprite;
            }
            else
            {
                spawnedImages[i].sprite = images[i].normalSprite;
            }
        }
    }
}