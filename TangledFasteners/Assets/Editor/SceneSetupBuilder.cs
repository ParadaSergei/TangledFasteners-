using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TangledFasteners;

namespace TangledFasteners.Editor
{
    public class SceneSetupBuilder
    {
        [MenuItem("TangledFasteners/Build Game Scene")]
        public static void BuildScene()
        {
            var scene = EditorSceneManager.GetActiveScene();

            // 1. Setup Camera & Light
            Camera mainCam = Camera.main;
            if (mainCam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                mainCam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            mainCam.transform.position = new Vector3(-2f, 8f, -10f);
            mainCam.transform.rotation = Quaternion.Euler(40f, 10f, 0f);
            mainCam.orthographic = true;
            mainCam.orthographicSize = 6.5f;
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.2f, 0.21f, 0.23f);

            Light dirLight = Object.FindFirstObjectByType<Light>();
            if (dirLight == null)
            {
                GameObject lightObj = new GameObject("Directional Light");
                dirLight = lightObj.AddComponent<Light>();
                dirLight.type = LightType.Directional;
            }
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // 2. Load or Create TMP Font Asset
            string fontAssetPath = "Assets/TextMesh Pro/Resources/Fonts & Materials/RuslanDisplay-Regular SDF.asset";
            TMP_FontAsset ruslanFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
            if (ruslanFontAsset == null)
            {
                Font ruslanFont = AssetDatabase.LoadAssetAtPath<Font>("Assets/TextMesh Pro/Fonts/RuslanDisplay-Regular.ttf");
                if (ruslanFont != null)
                {
                    ruslanFontAsset = TMP_FontAsset.CreateFontAsset(ruslanFont);
                    ruslanFontAsset.name = "RuslanDisplay-Regular SDF";
                    AssetDatabase.CreateAsset(ruslanFontAsset, fontAssetPath);

                    if (ruslanFontAsset.material != null)
                    {
                        ruslanFontAsset.material.name = ruslanFontAsset.name + " Material";
                        AssetDatabase.AddObjectToAsset(ruslanFontAsset.material, ruslanFontAsset);
                    }
                    if (ruslanFontAsset.atlasTexture != null)
                    {
                        ruslanFontAsset.atlasTexture.name = ruslanFontAsset.name + " Atlas";
                        AssetDatabase.AddObjectToAsset(ruslanFontAsset.atlasTexture, ruslanFontAsset);
                    }
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    ruslanFontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
                }
            }

            Sprite nextButtonSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI_Package/Casual_UI_Slicing/Win_Panel/next ibutton.png");
            Sprite retryButtonSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI_Package/Casual_UI_Slicing/Win_Panel/retry button.png");
            Sprite soundOnSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI_Package/Casual_UI_Slicing/Icons/sound on.png");
            Sprite soundOffSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI_Package/Casual_UI_Slicing/Icons/sound off.png");
            Sprite ribbonSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/UI_Package/Casual_UI_Slicing/Win_Panel/ribbon.png");

            // 3. Setup Managers
            GameObject managers = GameObject.Find("Managers");
            if (managers == null)
            {
                managers = new GameObject("Managers");
            }
            NutSortManager nsm = managers.GetComponent<NutSortManager>() ?? managers.AddComponent<NutSortManager>();
            LevelGenerator gen = managers.GetComponent<LevelGenerator>() ?? managers.AddComponent<LevelGenerator>();
            gen.boltPrefab = Resources.Load<GameObject>("Prefabs/Bolt");
            gen.nutPrefab = Resources.Load<GameObject>("Prefabs/Gaika");

            LevelManager lm = managers.GetComponent<LevelManager>() ?? managers.AddComponent<LevelManager>();
            AudioManager am = managers.GetComponent<AudioManager>() ?? managers.AddComponent<AudioManager>();
            InputController input = managers.GetComponent<InputController>() ?? managers.AddComponent<InputController>();
            input.mainCamera = mainCam;
            lm.levelGenerator = gen;

            // 4. Setup Canvas & UI
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj == null)
            {
                canvasObj = new GameObject("Canvas");
            }
            Canvas canvas = canvasObj.GetComponent<Canvas>() ?? canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>() ?? canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            if (canvasObj.GetComponent<GraphicRaycaster>() == null)
            {
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            UIManager uiManager = canvasObj.GetComponent<UIManager>() ?? canvasObj.AddComponent<UIManager>();
            uiManager.soundOnSprite = soundOnSprite;
            uiManager.soundOffSprite = soundOffSprite;

            // Top Bar
            Transform existingTopBar = canvasObj.transform.Find("TopBar");
            if (existingTopBar != null)
            {
                Object.DestroyImmediate(existingTopBar.gameObject);
            }

            GameObject topBar = new GameObject("TopBar");
            topBar.transform.SetParent(canvasObj.transform, false);

            RectTransform topBarRect = topBar.AddComponent<RectTransform>();
            topBarRect.anchorMin = new Vector2(0, 1);
            topBarRect.anchorMax = new Vector2(1, 1);
            topBarRect.pivot = new Vector2(0.5f, 1);
            topBarRect.anchoredPosition = new Vector2(0, -40);
            topBarRect.sizeDelta = new Vector2(0, 180);

            // Level Text (TMP)
            GameObject levelTextObj = new GameObject("LevelText");
            levelTextObj.transform.SetParent(topBar.transform, false);
            TextMeshProUGUI levelText = levelTextObj.AddComponent<TextMeshProUGUI>();
            if (ruslanFontAsset != null) levelText.font = ruslanFontAsset;
            levelText.fontSize = 54;
            levelText.alignment = TextAlignmentOptions.Center;
            levelText.color = new Color(1f, 0.92f, 0.3f); // Bright friendly gold
            levelText.text = "Уровень 1";
            uiManager.levelText = levelText;

            RectTransform ltRect = levelTextObj.GetComponent<RectTransform>();
            ltRect.anchoredPosition = new Vector2(0, 30);
            ltRect.sizeDelta = new Vector2(500, 70);

            // Moves Text (TMP)
            GameObject movesTextObj = new GameObject("MovesText");
            movesTextObj.transform.SetParent(topBar.transform, false);
            TextMeshProUGUI movesText = movesTextObj.AddComponent<TextMeshProUGUI>();
            if (ruslanFontAsset != null) movesText.font = ruslanFontAsset;
            movesText.fontSize = 42;
            movesText.alignment = TextAlignmentOptions.Center;
            movesText.color = Color.white;
            movesText.text = "Ходы: 0";
            uiManager.movesText = movesText;

            RectTransform mtRect = movesTextObj.GetComponent<RectTransform>();
            mtRect.anchoredPosition = new Vector2(0, -35);
            mtRect.sizeDelta = new Vector2(500, 60);

            // Restart Button (uses retry button sprite from UI_Package)
            GameObject restartBtnObj = CreateButton(
                topBar.transform,
                "RestartButton",
                "Рестарт",
                new Vector2(-380, 0),
                new Vector2(180, 90),
                retryButtonSprite,
                ruslanFontAsset,
                28
            );
            uiManager.restartButton = restartBtnObj.GetComponent<Button>();
            uiManager.restartButtonImage = restartBtnObj.GetComponent<Image>();
            uiManager.restartButtonText = restartBtnObj.GetComponentInChildren<TextMeshProUGUI>();

            // Language Button
            GameObject langBtnObj = CreateButton(
                topBar.transform,
                "LanguageButton",
                "Язык: RU",
                new Vector2(190, 0),
                new Vector2(180, 90),
                retryButtonSprite,
                ruslanFontAsset,
                24
            );
            uiManager.languageButton = langBtnObj.GetComponent<Button>();
            uiManager.languageButtonImage = langBtnObj.GetComponent<Image>();
            uiManager.languageButtonText = langBtnObj.GetComponentInChildren<TextMeshProUGUI>();

            // Sound Button (uses sound on / sound off sprite from UI_Package)
            GameObject soundBtnObj = CreateButton(
                topBar.transform,
                "SoundButton",
                "Звук: ВКЛ",
                new Vector2(380, 0),
                new Vector2(180, 90),
                soundOnSprite,
                ruslanFontAsset,
                24
            );
            uiManager.soundButton = soundBtnObj.GetComponent<Button>();
            uiManager.soundButtonImage = soundBtnObj.GetComponent<Image>();
            uiManager.soundButtonText = soundBtnObj.GetComponentInChildren<TextMeshProUGUI>();

            // Level Complete Panel
            Transform existingWinPanel = canvasObj.transform.Find("LevelCompletePanel");
            if (existingWinPanel != null)
            {
                Object.DestroyImmediate(existingWinPanel.gameObject);
            }

            GameObject winPanel = new GameObject("LevelCompletePanel");
            winPanel.transform.SetParent(canvasObj.transform, false);

            RectTransform winRect = winPanel.AddComponent<RectTransform>();
            winRect.anchorMin = Vector2.zero;
            winRect.anchorMax = Vector2.one;
            winRect.sizeDelta = Vector2.zero;

            Image winBg = winPanel.AddComponent<Image>();
            winBg.color = new Color(0, 0, 0, 0.75f);

            // Ribbon Image Header (Behind win text)
            GameObject ribbonObj = new GameObject("RibbonHeader");
            ribbonObj.transform.SetParent(winPanel.transform, false);

            Image ribbonImg = ribbonObj.AddComponent<Image>();
            if (ribbonSprite != null) ribbonImg.sprite = ribbonSprite;
            ribbonImg.preserveAspect = true;

            RectTransform ribbonRect = ribbonObj.GetComponent<RectTransform>();
            ribbonRect.anchoredPosition = new Vector2(0, 200);
            ribbonRect.sizeDelta = new Vector2(800, 250);
            uiManager.winRibbonImage = ribbonImg;

            // Win Title Text ("Уровень пройден") ON TOP of Ribbon Background
            GameObject winTextObj = new GameObject("WinText");
            winTextObj.transform.SetParent(ribbonObj.transform, false);
            TextMeshProUGUI winText = winTextObj.AddComponent<TextMeshProUGUI>();
            if (ruslanFontAsset != null) winText.font = ruslanFontAsset;
            winText.fontSize = 52;
            winText.alignment = TextAlignmentOptions.Center;
            winText.color = new Color(1f, 0.95f, 0.8f);
            winText.text = "Уровень пройден";
            uiManager.winTitleText = winText;

            RectTransform wtRect = winTextObj.GetComponent<RectTransform>();
            wtRect.anchorMin = Vector2.zero;
            wtRect.anchorMax = Vector2.one;
            wtRect.sizeDelta = Vector2.zero;
            wtRect.anchoredPosition = new Vector2(0, 10);

            // Continue Button (uses next ibutton sprite from UI_Package)
            GameObject continueBtnObj = CreateButton(
                winPanel.transform,
                "ContinueButton",
                "Продолжить",
                new Vector2(0, -150),
                new Vector2(320, 120),
                nextButtonSprite,
                ruslanFontAsset,
                36
            );
            uiManager.continueButton = continueBtnObj.GetComponent<Button>();
            uiManager.continueButtonImage = continueBtnObj.GetComponent<Image>();
            uiManager.continueButtonText = continueBtnObj.GetComponentInChildren<TextMeshProUGUI>();
            uiManager.levelCompletePanel = winPanel;

            // Event System for UI interactions
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            // Generate first level
            gen.GenerateLevel(1);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("Nut & Bolt Sorting Scene Built Successfully with kids UI!");
        }

        private static GameObject CreateButton(
            Transform parent,
            string name,
            string label,
            Vector2 position,
            Vector2 size,
            Sprite buttonSprite,
            TMP_FontAsset fontAsset,
            float fontSize
        )
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            Image img = btnObj.AddComponent<Image>();
            if (buttonSprite != null)
            {
                img.sprite = buttonSprite;
                img.type = Image.Type.Simple;
                img.preserveAspect = true;
            }
            else
            {
                img.color = new Color(0.2f, 0.6f, 0.9f);
            }

            Button btn = btnObj.AddComponent<Button>();

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);

            TextMeshProUGUI txt = textObj.AddComponent<TextMeshProUGUI>();
            if (fontAsset != null) txt.font = fontAsset;
            txt.fontSize = fontSize;
            txt.alignment = TextAlignmentOptions.Center;
            txt.color = Color.white;
            txt.text = label;

            RectTransform tRect = textObj.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.sizeDelta = Vector2.zero;

            return btnObj;
        }
    }
}
