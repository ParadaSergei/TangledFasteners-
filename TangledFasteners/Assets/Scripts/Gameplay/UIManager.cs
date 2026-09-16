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
        public Button soundButton;
        public Image soundButtonImage;
        public Sprite soundOnSprite;
        public Sprite soundOffSprite;
        public TextMeshProUGUI soundButtonText;

        [Header("Victory Popup UI")]
        public GameObject levelCompletePanel;
        public Image winRibbonImage;
        public TextMeshProUGUI winTitleText;
        public Button continueButton;
        public Image continueButtonImage;

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

            if (continueButton != null)
            {
                continueButton.onClick.RemoveAllListeners();
                continueButton.onClick.AddListener(OnContinueClicked);
            }

            if (levelCompletePanel != null)
                levelCompletePanel.SetActive(false);

            if (AudioManager.Instance != null)
            {
                UpdateSoundButtonUI(AudioManager.Instance.IsMuted);
            }
        }

        public void UpdateLevelText(int level)
        {
            if (levelText != null)
            {
                levelText.text = $"Уровень {level}";
            }
        }

        public void UpdateMoveCount(int moves)
        {
            if (movesText != null)
            {
                movesText.text = $"Ходы: {moves}";
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
                soundButtonText.text = isMuted ? "Звук: ВЫКЛ" : "Звук: ВКЛ";
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
