using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace TangledFasteners
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        public LevelGenerator levelGenerator;
        [SerializeField] private GameObject buttonSound;
        [SerializeField] private Sprite buttonSpriteOn;
        [SerializeField] private Sprite buttonSpriteOff;

        public AudioSource audio;

        public int currentLevel = 1;
        public string currentLanguage = "ru";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            audio = gameObject.GetComponent<AudioSource>();
            DetectLanguage();
            LoadCurrentLevel();
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern string GetYandexLanguage_js();
#endif

        private void DetectLanguage()
        {
            string detectedLang = "ru";

#if UNITY_WEBGL && !UNITY_EDITOR
            try
            {
                string sdkLang = GetYandexLanguage_js();
                if (!string.IsNullOrEmpty(sdkLang))
                {
                    detectedLang = sdkLang.ToLower();
                }
            }
            catch
            {
                detectedLang = Application.systemLanguage.ToString().ToLower();
            }
#else
            detectedLang = Application.systemLanguage.ToString().ToLower();
            try
            {
                var field = typeof(YG.YG2).GetField("lang", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (field != null)
                {
                    string ygLang = field.GetValue(null) as string;
                    if (!string.IsNullOrEmpty(ygLang))
                    {
                        detectedLang = ygLang.ToLower();
                    }
                }
            }
            catch { }
#endif

            if (detectedLang.StartsWith("ru") || detectedLang.Contains("russian") ||
                detectedLang.StartsWith("be") || detectedLang.StartsWith("uk") ||
                detectedLang.StartsWith("kk") || detectedLang.StartsWith("uz"))
            {
                currentLanguage = "ru";
            }
            else
            {
                currentLanguage = "en";
            }

            UIManager.Instance?.UpdateAllTexts();
        }

        public void LoadCurrentLevel()
        {
            Time.timeScale = 1f;

            if (levelGenerator == null)
            {
                levelGenerator = FindFirstObjectByType<LevelGenerator>();
            }

            if (levelGenerator != null)
            {
                levelGenerator.GenerateLevel(currentLevel);
                UIManager.Instance?.UpdateLevelText(currentLevel);
            }
        }
        public void SoundPicture()
        {
            Image buttonImage = buttonSound.GetComponent<Image>();
            if(audio.mute == true) buttonImage.sprite = buttonSpriteOn;
            else buttonImage.sprite = buttonSpriteOff;
        }

        public void RestartLevel()
        {
            LoadCurrentLevel();
            UIManager.Instance?.HideLevelCompletePanel();
        }

        private bool isChangingLevel;

        public void NextLevel()
        {
            if (isChangingLevel) return;
            isChangingLevel = true;

            currentLevel++;
            LoadCurrentLevel();
            UIManager.Instance?.HideLevelCompletePanel();

#if InterstitialAdv_yg
            if (currentLevel % 2 == 0)
            {
                YG.YG2.InterstitialAdvShow();
            }
#endif

            isChangingLevel = false;
        }

        public void LoadSceneByName(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        public void ChangeLanguage()
        {
            currentLanguage = (currentLanguage == "ru") ? "en" : "ru";
            UIManager.Instance?.UpdateAllTexts();
        }

        public void ToggleLanguage()
        {
            ChangeLanguage();
        }

        public void ChangeLanguage(string lang)
        {
            if (lang == "ru" || lang == "en")
            {
                currentLanguage = lang;
                UIManager.Instance?.UpdateAllTexts();
            }
        }
    }
}
