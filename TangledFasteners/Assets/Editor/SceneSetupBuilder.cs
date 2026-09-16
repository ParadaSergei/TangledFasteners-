using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
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
            mainCam.transform.position = new Vector3(0, 0, -10f);
            mainCam.transform.rotation = Quaternion.identity;
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

            // 2. Setup Managers
            GameObject managers = new GameObject("Managers");
            NutSortManager nsm = managers.AddComponent<NutSortManager>();
            LevelGenerator gen = managers.AddComponent<LevelGenerator>();
            gen.boltPrefab = Resources.Load<GameObject>("Prefabs/Bolt");
            gen.nutPrefab = Resources.Load<GameObject>("Prefabs/Gaika");

            LevelManager lm = managers.AddComponent<LevelManager>();
            AudioManager am = managers.AddComponent<AudioManager>();
            InputController input = managers.AddComponent<InputController>();
            input.mainCamera = mainCam;
            lm.levelGenerator = gen;

            // 3. Setup Canvas & UI
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            UIManager uiManager = canvasObj.AddComponent<UIManager>();

            // Top Bar
            GameObject topBar = new GameObject("TopBar");
            topBar.transform.SetParent(canvasObj.transform, false);

            RectTransform topBarRect = topBar.AddComponent<RectTransform>();
            topBarRect.anchorMin = new Vector2(0, 1);
            topBarRect.anchorMax = new Vector2(1, 1);
            topBarRect.pivot = new Vector2(0.5f, 1);
            topBarRect.anchoredPosition = new Vector2(0, -20);
            topBarRect.sizeDelta = new Vector2(0, 100);

            // Level Text
            GameObject levelTextObj = new GameObject("LevelText");
            levelTextObj.transform.SetParent(topBar.transform, false);
            Text levelText = levelTextObj.AddComponent<Text>();
            levelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            levelText.fontSize = 32;
            levelText.alignment = TextAnchor.MiddleCenter;
            levelText.color = Color.black;
            levelText.text = "Уровень 1";
            uiManager.levelText = levelText;

            RectTransform ltRect = levelTextObj.GetComponent<RectTransform>();
            ltRect.anchoredPosition = Vector2.zero;
            ltRect.sizeDelta = new Vector2(300, 60);

            // Restart Button
            GameObject restartBtnObj = CreateButton(topBar.transform, "RestartButton", "Рестарт", new Vector2(-200, 0), new Vector2(140, 50));
            uiManager.restartButton = restartBtnObj.GetComponent<Button>();

            // Sound Button
            GameObject soundBtnObj = CreateButton(topBar.transform, "SoundButton", "Звук: ВКЛ", new Vector2(200, 0), new Vector2(160, 50));
            uiManager.soundButton = soundBtnObj.GetComponent<Button>();
            uiManager.soundButtonText = soundBtnObj.GetComponentInChildren<Text>();

            // Level Complete Panel
            GameObject winPanel = new GameObject("LevelCompletePanel");
            winPanel.transform.SetParent(canvasObj.transform, false);

            RectTransform winRect = winPanel.AddComponent<RectTransform>();
            winRect.anchorMin = Vector2.zero;
            winRect.anchorMax = Vector2.one;
            winRect.sizeDelta = Vector2.zero;

            Image winBg = winPanel.AddComponent<Image>();
            winBg.color = new Color(0, 0, 0, 0.75f);

            GameObject winTextObj = new GameObject("WinText");
            winTextObj.transform.SetParent(winPanel.transform, false);
            Text winText = winTextObj.AddComponent<Text>();
            winText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            winText.fontSize = 42;
            winText.alignment = TextAnchor.MiddleCenter;
            winText.color = Color.gold;
            winText.text = "Уровень Пройден!";

            RectTransform wtRect = winTextObj.GetComponent<RectTransform>();
            wtRect.anchoredPosition = new Vector2(0, 100);
            wtRect.sizeDelta = new Vector2(500, 100);

            GameObject continueBtnObj = CreateButton(winPanel.transform, "ContinueButton", "Продолжить", new Vector2(0, -50), new Vector2(220, 70));
            uiManager.continueButton = continueBtnObj.GetComponent<Button>();
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
            Debug.Log("Nut & Bolt Sorting Scene Built Successfully!");
        }

        private static GameObject CreateButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.6f, 0.9f);

            Button btn = btnObj.AddComponent<Button>();

            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);

            Text txt = textObj.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 20;
            txt.alignment = TextAnchor.MiddleCenter;
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
