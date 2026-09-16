using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TangledFasteners
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Top Bar UI")]
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI movesText;
        public Button restartButton;
        public Image restartButtonImage;
        public TextMeshProUGUI restartButtonText;
        public Button soundButton;
        public Image soundButtonImage;
        public Sprite soundOnSprite;
        public Sprite soundOffSprite;
        public TextMeshProUGUI soundButtonText;
        public Button languageButton;
        public Image languageButtonImage;
        public TextMeshProUGUI languageButtonText;

        [Header("Victory Popup UI")]
        public GameObject levelCompletePanel;
        public Image winRibbonImage;
        public TextMeshProUGUI winTitleText;
        public Button continueButton;
        public Image continueButtonImage;
        public TextMeshProUGUI continueButtonText;

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
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartClicked);
            }

            if (soundButton != null)
            {
                soundButton.onClick.RemoveAllListeners();
                soundButton.onClick.AddListener(OnSoundToggleClicked);
            }

            if (languageButton != null)
            {
                languageButton.onClick.RemoveAllListeners();
                languageButton.onClick.AddListener(OnLanguageToggleClicked);
            }

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueClicked);
            }

            if (levelCompletePanel != null)
                levelCompletePanel.SetActive(false);

            UpdateAllTexts();
        }

        public void UpdateAllTexts()
        {
            bool isRu = LevelManager.Instance == null || LevelManager.Instance.currentLanguage == "ru";

            int level = LevelManager.Instance != null ? LevelManager.Instance.currentLevel : 1;
            UpdateLevelText(level);

            int moves = NutSortManager.Instance != null ? NutSortManager.Instance.moveCount : 0;
            UpdateMoveCount(moves);

            if (AudioManager.Instance != null)
            {
                UpdateSoundButtonUI(AudioManager.Instance.IsMuted);
            }
            else if (soundButtonText != null)
            {
                soundButtonText.text = isRu ? "Звук: ВКЛ" : "Sound: ON";
            }

            if (restartButtonText == null && restartButton != null)
            {
                restartButtonText = restartButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (restartButtonText != null)
            {
                restartButtonText.text = isRu ? "Рестарт" : "Restart";
            }

            if (winTitleText != null)
            {
                winTitleText.text = isRu ? "Уровень пройден" : "Level Completed";
            }

            if (continueButtonText == null && continueButton != null)
            {
                continueButtonText = continueButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (continueButtonText != null)
            {
                continueButtonText.text = isRu ? "Продолжить" : "Continue";
            }

            if (languageButtonText == null && languageButton != null)
            {
                languageButtonText = languageButton.GetComponentInChildren<TextMeshProUGUI>();
            }
            if (languageButtonText != null)
            {
                languageButtonText.text = isRu ? "Язык: RU" : "Lang: EN";
            }
        }

        public void UpdateLevelText(int level)
        {
            if (levelText != null)
            {
                bool isRu = LevelManager.Instance == null || LevelManager.Instance.currentLanguage == "ru";
                levelText.text = isRu ? $"Уровень {level}" : $"Level {level}";
            }
        }

        public void UpdateMoveCount(int moves)
        {
            if (movesText != null)
            {
                bool isRu = LevelManager.Instance == null || LevelManager.Instance.currentLanguage == "ru";
                movesText.text = isRu ? $"Ходы: {moves}" : $"Moves: {moves}";
            }
        }

        public void UpdateSoundButtonUI(bool isMuted)
        {
            if (soundButtonImage != null)
            {
                if (isMuted && soundOffSprite != null)
                {
                    soundButtonImage.sprite = soundOffSprite;
                }
                else if (!isMuted && soundOnSprite != null)
                {
                    soundButtonImage.sprite = soundOnSprite;
                }
            }

            if (soundButtonText != null)
            {
                bool isRu = LevelManager.Instance == null || LevelManager.Instance.currentLanguage == "ru";
                soundButtonText.text = isRu ? (isMuted ? "Звук: ВЫКЛ" : "Звук: ВКЛ") : (isMuted ? "Sound: OFF" : "Sound: ON");
            }
        }

        public void ShowLevelCompletePanel()
        {
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
            }
            Time.timeScale = 0f;
        }

        public void HideLevelCompletePanel()
        {
            Time.timeScale = 1f;
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(false);
            }
        }

        private void OnRestartClicked()
        {
            LevelManager.Instance?.RestartLevel();
        }

        private void OnSoundToggleClicked()
        {
            AudioManager.Instance?.ToggleSound();
        }

        private void OnLanguageToggleClicked()
        {
            LevelManager.Instance?.ChangeLanguage();
        }

        private void OnContinueClicked()
        {
            if (levelCompletePanel != null && !levelCompletePanel.activeSelf)
            {
                return;
            }
            HideLevelCompletePanel();
            LevelManager.Instance?.NextLevel();
        }
    }
}
